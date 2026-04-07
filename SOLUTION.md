# Solution Design

## Overview

ProductCatalogManager is a full-stack textbook catalog application. The backend is a .NET 10 ASP.NET Core Web API and the frontend is an Angular 21 SPA. The two communicate over HTTP; no authentication is required.

---

## Project Structure

```
ProductCatalogManager/
├── ProductCatalogManager.API        # HTTP layer: controllers, filters, middleware, serialization
├── ProductCatalogManager.Domain     # Core domain: repositories, DTOs, cache, search engine
├── ProductCatalogManager.Queries    # LINQ query extensions and query-parameter models
├── ProductCatalogManager.Utilities  # Reusable, domain-agnostic utilities (search scoring)
└── ProductCatalogManager.UI         # Angular SPA
```

The layering rule is: API → Domain → Utilities. Queries is a shared library used by API; it has no dependency on Domain internals.

### Central Package Management

Two MSBuild props files apply to every project in the solution:

- `Directory.Build.props` — sets `TargetFramework`, `Nullable`, and `ImplicitUsings` globally so individual `.csproj` files do not repeat them.
- `Directory.Packages.props` — enables **Central Package Management (CPM)**. All NuGet package versions are declared once here; individual `.csproj` files reference packages with `<PackageReference Include="..." />` only, without a `Version` attribute.

CPM means there is a single place to audit or update a dependency across the whole solution. Without it, five projects each pinning their own version of `Microsoft.EntityFrameworkCore` would risk silent version mismatches.

---

## Backend

### Storage

All data lives in a `ConcurrentDictionary<int, T>` inside the generic `Repository<T>` base class. There is no external database. Entity Framework Core (InMemory provider) is registered in the DI container and `CatalogDbContext` is defined, but persistence is handled entirely by the repositories — EF Core is present as a foundation for a future migration to a real database without changing the repository interfaces.

**Trade-off:** Losing data on process restart is intentional for this scope. Resetting the catalog is as simple as restarting the API. Moving to SQL Server or PostgreSQL later only requires swapping the `IRepository<T>` implementations and updating `Program.cs`.

### Repository Pattern

`Repository<T>` implements `IRepository<T>` (CRUD) and manages auto-incrementing IDs with `Interlocked.Increment`. Concrete repositories (`ProductRepository`, `CategoryRepository`) extend it and add domain-specific queries (`GetByCategoryIdAsync`, `GetBySkuAsync`, `GetTreeAsync`).

`CategoryRepository.GetTreeAsync` builds the recursive tree in memory by walking parent–child relationships. This is efficient at the current catalog scale; a deeper hierarchy or very large category set would warrant a CTE query instead.

### Caching

`CacheLayer` is a lightweight dictionary-backed cache. Two keyed singleton instances are registered — one for products, one for categories — so the groups can be invalidated independently. Cache invalidation is all-or-nothing per group: any write to products evicts all product cache entries.

**Trade-off:** Granular per-key invalidation would reduce cache misses after a single write, but the implementation complexity is not justified at this scale. The current approach is easy to reason about and avoids stale-entry bugs.

### Product Filtering vs. Search

There are two intentionally distinct query paths:

| Endpoint | Mechanism | Use case |
|---|---|---|
| `GET /api/products?search=` | LINQ `Contains` (case-insensitive) | Structured list page with filters, sort and pagination |
| `GET /api/products/search` | Weighted fuzzy `SearchEngineUtility<T>` | Free-text relevance search |

Keeping them separate means the list page has predictable, stable ordering and pagination, while the search endpoint can rank results by relevance without disrupting paging logic.

### Search Engine (`ProductCatalogManager.Utilities`)

`SearchEngineUtility<T>` is a zero-dependency, generic, multi-field weighted fuzzy search engine built on Levenshtein distance. Scoring tiers per field:

| Match type | Score |
|---|---|
| Exact | 1.00 |
| Starts-with | 0.85 |
| Contains | 0.70 |
| Fuzzy (similarity ≥ 0.6) | 0.60 × similarity |

Fields are weighted independently (`Name` = 2.0, `SKU` = 1.5, `Description` = 1.0). Multi-token queries require every token to match at least one field; the total score is the sum of each token's best weighted score.

**Trade-off:** A dedicated search service (Elasticsearch, Azure AI Search) would offer inverted indexes, stemming, and horizontal scale. For a self-contained demo with tens to low thousands of records, a pure-C# implementation avoids infrastructure dependencies and is straightforward to test.

Search results are cached via `CacheLayer` keyed by `(name, categoryId)`.

### Sorting

`ProductComparers` defines static `IComparer<ProductDto>` instances for every supported sort field and direction. After retrieving a cached filtered result set, `ProductsController` calls `ProductComparers.SortBy` before paginating.

**Trade-off:** Sorting is not part of the cache key, so the same filtered list is reused regardless of requested sort order. This improves cache hit rate but means an in-memory sort on every request. For small-to-medium result sets this is negligible; at large scale you would want to push sort into the query layer.

### Validation

`ValidationFilter` (an `IAsyncActionFilter`) intercepts every request. It first checks model binding errors, then resolves the appropriate `IValidator<T>` from the DI container and runs FluentValidation. Both sources produce a consistent `ValidationProblemDetails` 400 response. The default ASP.NET Core model-state filter is suppressed so all validation flows through a single path.

### Request Auditing

`RequestAuditMiddleware` wraps every request with a `Stopwatch`. Elapsed time is written to the `X-Response-Time-Ms` response header and logged as a structured log entry (`{Method} {Path} {StatusCode} {ElapsedMs}`).

### JSON Serialization

`CategoryTreeNodeConverter` is a custom `JsonConverter<CategoryTreeNode>` that:
- Uses snake_case property names.
- Omits `parent_category_id` when `null` (root nodes).
- Omits `children` when empty (leaf nodes).

This keeps the wire format compact and idiomatic for the Angular client.

---

## Frontend

### State Management

`ProductService` and `CategoryService` use `BehaviorSubject` as a lightweight reactive store. Components subscribe to `products$`, `loading$`, `totalCount$` and `totalPages$` observables. No NgRx or external state library is used.

**Trade-off:** `BehaviorSubject` is sufficient for a two-page SPA with no shared cross-route state complexity. NgRx would add meaningful overhead for the current feature set.

### API Communication

`environment.ts` holds the base API URL (`http://localhost:5194`). All HTTP calls go through the two services. After a mutation (create), the service reloads the current query to keep the list in sync rather than applying optimistic updates, trading a network round-trip for simplicity and correctness.

### Structure

| Folder | Contents |
|---|---|
| `pages/` | Route-level components (`ProductsPage`, `CategoriesPage`) |
| `components/` | Reusable UI components (`ProductList`, `ProductForm`, `SearchBar`, `CategoryFilter`, `ConfirmDialog`) |
| `services/` | `ProductService`, `CategoryService` |
| `models/` | TypeScript interfaces mirroring API contracts |

---

## What Would Change for Production

- Replace in-memory repositories with a real database (the `IRepository<T>` interfaces are stable; only implementations change).
- Add authentication and authorization (API keys or JWT).
- Replace `CacheLayer` with `IMemoryCache` or a distributed cache (Redis) to survive restarts and support multiple API instances.
- Add structured error handling middleware returning RFC 9457 problem details for all unhandled exceptions.
- Introduce unit and integration tests (currently noted as a TODO).

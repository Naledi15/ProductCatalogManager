# ProductCatalogManager

A full-stack product catalog application built with .NET 10 (ASP.NET Core Web API) and Angular 21.

---

## Prerequisites

| Tool | Version |
|------|---------|
| [.NET SDK](https://dotnet.microsoft.com/download) | 10.0+ |
| [Node.js](https://nodejs.org/) | LTS (20+) |
| [npm](https://www.npmjs.com/) | 11.9.0+ |
| [Angular CLI](https://angular.dev/tools/cli) | 21+ |

---

## API

All commands run from the API project directory:

```bash
cd src/ProductCatalogManager.Server/ProductCatalogManager.API
```

### Restore packages

```bash
dotnet restore
```

### Build

```bash
dotnet build
```

### Test

> Unit tests are not available yet — coming soon.

### Run

```bash
dotnet run
```

The API starts on:

| Scheme | URL |
|--------|-----|
| HTTP | <http://localhost:5194> |
| HTTPS | <https://localhost:7120> |

The in-memory database is seeded automatically on startup — no external database is required.

#### API reference (Scalar)

With the API running, open <http://localhost:5194/scalar/v1> in your browser for the interactive API reference.

---

## UI

All commands run from `src/ProductCatalogManager.Client/ProductCatalogManager.UI`.

```bash
cd src/ProductCatalogManager.Client/ProductCatalogManager.UI
```

### Install dependencies

```bash
npm install
```

### Build

```bash
npm run build
```

### Test

```bash
npm test
```

### Run (dev server)

```bash
npm start
```

The UI is served at <http://localhost:4200> and proxies API calls to `http://localhost:5194`.

> **Note:** Make sure the API is running before starting the UI. The API uses an in-memory database seeded on startup — to reset or refresh data, stop the API and run it again.

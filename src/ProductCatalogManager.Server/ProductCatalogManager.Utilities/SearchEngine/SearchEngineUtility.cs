namespace ProductCatalogManager.Utilities.SearchEngine;

/// <summary>
/// Generic in-memory search engine with fuzzy matching and multi-field weighted scoring.
///
/// Scoring tiers (per field, normalised to [0, 1] then multiplied by field weight):
///   1.00 — exact match
///   0.85 — field value starts with the query token
///   0.70 — field value contains the query token
///   0.60 × similarity — fuzzy (Levenshtein) match with similarity ≥ MinFuzzySimilarity
///
/// Multi-token queries (space-separated): every token must match at least one field;
/// the total score is the sum of each token's best weighted field score.
/// </summary>
public sealed class SearchEngineUtility<T>
{
    private const double ExactScore         = 1.00;
    private const double StartsWithScore    = 0.85;
    private const double ContainsScore      = 0.70;
    private const double FuzzyBaseScore     = 0.60;
    private const double MinFuzzySimilarity = 0.60;  // ~2 edits in a 5-char word

    private readonly IReadOnlyList<SearchField<T>> _fields;

    public SearchEngineUtility(IReadOnlyList<SearchField<T>> fields)
    {
        if (fields is null || fields.Count == 0)
            throw new ArgumentException("At least one search field must be provided.", nameof(fields));
        _fields = fields;
    }

    /// <summary>
    /// Searches <paramref name="source"/> for items matching <paramref name="query"/>,
    /// returning results sorted by descending relevance score.
    /// </summary>
    /// <param name="source">Items to search.</param>
    /// <param name="query">Free-text query; tokens are split by whitespace.</param>
    /// <param name="maxResults">Maximum number of results to return.</param>
    public IReadOnlyList<SearchResult<T>> Search(
        IEnumerable<T> source,
        string query,
        int maxResults = 50)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (string.IsNullOrWhiteSpace(query))
            return source.Take(maxResults)
                         .Select(item => new SearchResult<T>(item, 0))
                         .ToList();

        var tokens = query.ToLowerInvariant()
                          .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var results = new List<SearchResult<T>>();

        foreach (var item in source)
        {
            double totalScore = ScoreItem(item, tokens);
            if (totalScore > 0)
                results.Add(new SearchResult<T>(item, totalScore));
        }

        results.Sort(static (a, b) => b.Score.CompareTo(a.Score));

        return results.Count <= maxResults ? results : results.GetRange(0, maxResults);
    }
    
    private double ScoreItem(T item, string[] tokens)
    {
        double total = 0;

        foreach (var token in tokens)
        {
            double bestForToken = 0;

            foreach (var field in _fields)
            {
                var raw = field.Extractor(item);
                if (string.IsNullOrEmpty(raw)) continue;

                double score = ScoreToken(raw.ToLowerInvariant(), token) * field.Weight;
                if (score > bestForToken)
                    bestForToken = score;
            }

            // All tokens are required — a zero score for any token eliminates the item.
            if (bestForToken == 0) return 0;

            total += bestForToken;
        }

        return total;
    }

    /// <summary>
    /// Returns 0..1 reflecting how well <paramref name="token"/> was found within
    /// the already-lowercased <paramref name="fieldValue"/>.
    /// </summary>
    private static double ScoreToken(string fieldValue, string token)
    {
        if (fieldValue == token)          return ExactScore;
        if (fieldValue.StartsWith(token)) return StartsWithScore;
        if (fieldValue.Contains(token))   return ContainsScore;

        // Fuzzy: compare the token against each individual word in the field value.
        // This means "lptop" can fuzzy-match the word "laptop" inside "laptop bag".
        double bestFuzzy = 0;
        foreach (var word in fieldValue.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            double similarity = Similarity(word, token);
            if (similarity >= MinFuzzySimilarity)
            {
                double candidate = FuzzyBaseScore * similarity;
                if (candidate > bestFuzzy) bestFuzzy = candidate;
            }
        }

        return bestFuzzy;
    }

    // ---------------------------------------------------------------------------
    // Levenshtein edit distance (two-row DP, O(m×n) time, O(min(m,n)) space)
    // ---------------------------------------------------------------------------

    /// <summary>
    /// Edit-distance similarity:  1 − distance / max(|s|, |t|)  ∈ [0, 1].
    /// </summary>
    private static double Similarity(string s, string t)
    {
        int maxLen = Math.Max(s.Length, t.Length);
        if (maxLen == 0) return 1.0;
        return 1.0 - (double)EditDistance(s.AsSpan(), t.AsSpan()) / maxLen;
    }

    private static int EditDistance(ReadOnlySpan<char> s, ReadOnlySpan<char> t)
    {
        // Keep s as the shorter span to minimise array allocation.
        if (s.Length > t.Length) { var tmp = s; s = t; t = tmp; }

        int sLen = s.Length, tLen = t.Length;
        if (sLen == 0) return tLen;
        if (tLen == 0) return sLen;

        int[] prev = new int[sLen + 1];
        int[] curr = new int[sLen + 1];

        for (int i = 0; i <= sLen; i++) prev[i] = i;

        for (int j = 1; j <= tLen; j++)
        {
            curr[0] = j;
            for (int i = 1; i <= sLen; i++)
            {
                int cost = s[i - 1] == t[j - 1] ? 0 : 1;
                curr[i] = Math.Min(
                    Math.Min(curr[i - 1] + 1,  // insert
                             prev[i]     + 1), // delete
                    prev[i - 1] + cost);        // substitute
            }
            (prev, curr) = (curr, prev);
        }

        return prev[sLen];
    }
}

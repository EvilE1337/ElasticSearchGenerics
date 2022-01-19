using Nest;
using System.Linq;

namespace FileSearch
{
    internal static class ElasticDescriptor
    {
        internal static SearchDescriptor<T> GetSearchDescriptor<T>(ElasticSearchArguments<T> ESA) where T : class
        {
            SearchDescriptor<T> searchDescriptor = new();

            if (ESA.Query != null)
                searchDescriptor.Query(q => GetQuery(ESA.Query));

            if (ESA.Highlight != null)
                searchDescriptor.Highlight(h => GetHighlite(ESA.Highlight));

            if (ESA.Suggest != null)
                searchDescriptor.Suggest(s => GetSuggest<T>(ESA.Suggest));

            return searchDescriptor;
        }

        #region Suggest

        private static SuggestContainerDescriptor<T> GetSuggest<T>(SuggestStruct<T> suggestData) where T : class
        {
            SuggestContainerDescriptor<T> suggestContainerDescriptor = new SuggestContainerDescriptor<T>();

            if (suggestData.Term != null && suggestData.Term.Field != null)
                suggestContainerDescriptor.Term(suggestData.Term.Name ?? "termSuggest", t => GetTerm<T>(suggestData.Term));

            if (suggestData.Completion != null)
                suggestContainerDescriptor.Completion(suggestData.Completion.Name ?? "completionSuggest", c => GetCompletion<T>(suggestData.Completion));

            if (suggestData.Phrase != null)
                suggestContainerDescriptor.Phrase(suggestData.Phrase.Name ?? "phraseSuggest", p => GetPhrase<T>(suggestData.Phrase));

            return suggestContainerDescriptor;
        }

        #region Term

        private static TermSuggesterDescriptor<T> GetTerm<T>(TermSuggest<T> termData) where T : class =>
            new TermSuggesterDescriptor<T>()
                    .MaxEdits(termData.MaxEdits)
                    .MaxInspections(termData.MaxInspections)
                    .MaxTermFrequency(termData.MaxTermFrequency)
                    .MinDocFrequency(termData.MinDocFrequency)
                    .MinWordLength(termData.MinWordLength)
                    .PrefixLength(termData.PrefixLength)
                    .SuggestMode((Elasticsearch.Net.SuggestMode?)termData.SuggestMode)
                    .Analyzer(termData.Analyzer)
                    .Field(termData.Field)
                    .ShardSize(termData.ShardSize)
                    .Size(termData.Size)
                    .Text(termData.Text)
                    .Sort((Nest.SuggestSort?)termData.Sort)
                    .StringDistance((Nest.StringDistance?)termData.StringDistance)
                    .LowercaseTerms(termData.LowercaseTerms);

        #endregion

        #region Completion
        private static CompletionSuggesterDescriptor<T> GetCompletion<T>(CompletionSuggest<T> completionData) where T : class
        {
            CompletionSuggesterDescriptor<T> completionSuggesterDescriptor = new CompletionSuggesterDescriptor<T>()
                    .Fuzzy(f => GetFuzziness(completionData.Fuzzy))
                    .Prefix(completionData.Prefix)
                    .Regex(completionData.Regex)
                    .Field(completionData.Field)
                    .SkipDuplicates(completionData.SkipDuplicates)
                    .Size(completionData.Size)
                    .Analyzer(completionData.Analyzer);

            foreach (CompletionContext<T> ctx in completionData.Contexts)
            {
                completionSuggesterDescriptor.Contexts(ctxs =>
                    ctxs.Context(ctx.Name,
                        c => GetContext(ctx)));
            };

            return completionSuggesterDescriptor;
        }

        private static SuggestContextQueryDescriptor<T> GetContext<T>(CompletionContext<T> ContextData) where T : class =>
            new SuggestContextQueryDescriptor<T>()
                .Context(ContextData.Context)
                .Boost(ContextData.Boost)
                .Prefix(ContextData.Prefix)
                .Precision(ContextData.Precision);

        private static SuggestFuzzinessDescriptor<T> GetFuzziness<T>(CompletionFuzzy<T> fuzzyData) where T : class =>
             new SuggestFuzzinessDescriptor<T>()
                .Fuzziness((Fuzziness)fuzzyData.Fuzziness)
                .MinLength(fuzzyData.MinLength)
                .PrefixLength(fuzzyData.PrefixLength)
                .Transpositions(fuzzyData.Transpositions)
                .UnicodeAware(fuzzyData.UnicodeAware);

        #endregion

        #region Phrase

        public static PhraseSuggestCollateQueryDescriptor GetCollateQuery(PhraseCollateQuery queryData) =>
            new PhraseSuggestCollateQueryDescriptor()
                .Id(queryData.Id)
                .Source(queryData.Source);

        private static PhraseSuggestCollateDescriptor<T> GetCollate<T>(PhraseCollate collateData) where T : class =>
            new PhraseSuggestCollateDescriptor<T>()
                .Params(collateData.Params)
                .Prune(collateData.Prune)
                .Query(q => GetCollateQuery(collateData.Query));

        private static DirectGeneratorDescriptor<T> GetDirectGenerator<T>(PhraseDirectGenerator<T> directData) where T : class =>
            new DirectGeneratorDescriptor<T>()
                .Field(directData.Field)
                .MaxEdits(directData.MaxEdits)
                .MaxInspections(directData.MaxInspections)
                .MaxTermFrequency(directData.MaxTermFrequency)
                .MinDocFrequency(directData.MinDocFrequency)
                .MinWordLength(directData.MinWordLength)
                .PrefixLength(directData.PrefixLength)
                .PostFilter(directData.PostFilter)
                .PreFilter(directData.PreFilter)
                .Size(directData.Size)
                .SuggestMode((Elasticsearch.Net.SuggestMode?)directData.SuggestMode);

        private static PhraseSuggestHighlightDescriptor GetHighlight(PhraseHighlight highlightData) =>
            new PhraseSuggestHighlightDescriptor()
                .PostTag(highlightData.PostTag)
                .PreTag(highlightData.PreTag);

        private static LaplaceSmoothingModelDescriptor GetSmoothingLaplace(PhraseLaplaceSmoothing laplaceData) =>
            new LaplaceSmoothingModelDescriptor()
                .Alpha(laplaceData.Alpha);

        private static LinearInterpolationSmoothingModelDescriptor GetSmoothingLinear(PhraseLinearSmoothing linearData) =>
            new LinearInterpolationSmoothingModelDescriptor()
                .BigramLambda(linearData.BigramLambda)
                .TrigramLambda(linearData.TrigramLambda)
                .UnigramLambda(linearData.UnigramLambda);

        private static StupidBackoffSmoothingModelDescriptor GetStuBack(PhraseStupidBackoffSmoothing stuData) =>
            new StupidBackoffSmoothingModelDescriptor()
                .Discount(stuData.Discount);

        private static SmoothingModelContainerDescriptor GetSmoothing(PhraseSmoothing smoothingData) =>
            new SmoothingModelContainerDescriptor()
                .Laplace(l => GetSmoothingLaplace(smoothingData.Laplace))
                .LinearInterpolation(l => GetSmoothingLinear(smoothingData.LinearInterpolation))
                .StupidBackoff(s => GetStuBack(smoothingData.StupidBackoff));

        private static PhraseSuggesterDescriptor<T> GetPhrase<T>(PhraseSuggest<T> termData) where T : class =>
            new PhraseSuggesterDescriptor<T>()
                .Size(termData.Size)
                .Analyzer(termData.Analyzer)
                .Field(termData.Field)
                .Text(termData.Text)
                .Collate(c => GetCollate<T>(termData.Collate))
                .Confidence(termData.Confidence)
                .DirectGenerator(d => GetDirectGenerator(termData.DirectGenerator))
                .ForceUnigrams(termData.ForceUnigrams)
                .GramSize(termData.GramSize)
                .Highlight(h => GetHighlight(termData.Highlight))
                .MaxErrors(termData.MaxErrors)
                .RealWordErrorLikelihood(termData.RealWordErrorLikelihood)
                .Separator(termData.Separator)
                .ShardSize(termData.ShardSize)
                .Smoothing(s => GetSmoothing(termData.Smoothing))
                .TokenLimit(termData.TokenLimit);

        #endregion

        #endregion

        #region Highlight

        private static HighlightDescriptor<T> GetHighlite<T>(HighlightStruct<T> highliteData) where T : class
        {
            HighlightDescriptor<T> Hdescription = new HighlightDescriptor<T>()
                .PreTags(highliteData.Tag ?? "")
                .PostTags(highliteData.Tag ?? "")
                .Encoder((Nest.HighlighterEncoder?)highliteData.Encoder)
                .BoundaryChars(highliteData.BoundaryChars)
                .BoundaryMaxScan(highliteData.BoundaryMaxScan)
                .BoundaryScanner((Nest.BoundaryScanner?)highliteData.BoundaryScanner)
                .BoundaryScannerLocale(highliteData.BoundaryScannerLocale)
                .Fragmenter((Nest.HighlighterFragmenter?)highliteData.Fragmenter)
                .FragmentOffset(highliteData.FragmentOffset)
                .FragmentSize(highliteData.FragmentSize)
                .HighlightQuery(q => GetQuery(highliteData.HighlightQuery))
                //.MaxAnalyzedOffset(highliteData.MaxAnalyzedOffset) use in new version
                .MaxFragmentLength(highliteData.MaxFragmentLength)
                .NoMatchSize(highliteData.NoMatchSize)
                .NumberOfFragments(highliteData.NumberOfFragments)
                .Order((Nest.HighlighterOrder?)highliteData.Order)
                .RequireFieldMatch(highliteData.RequireFieldMatch);

            if (highliteData.Fields != null)
                foreach (HighlightFieldDescriptor<T> field in
                    highliteData.Fields
                        .Where(w => w.Field != null)
                        .Select(s => GetFieldDescriptor(s)))
                {
                    Hdescription.Fields(fw => field);
                }

            return Hdescription;
        }

        private static HighlightFieldDescriptor<T> GetFieldDescriptor<T>(HighlightFields<T> fieldData) where T : class =>
            new HighlightFieldDescriptor<T>()
                .Field(fieldData.Field)
                .Type(fieldData.Type == null ? Nest.HighlighterType.Unified : (Nest.HighlighterType)fieldData.Type)
                .Fragmenter((Nest.HighlighterFragmenter?)fieldData.Fragmenter)
                .ForceSource(fieldData.ForceSource)
                .FragmentSize(fieldData.FragmentSize)
                .NumberOfFragments(fieldData.NumberOfFragments)
                .NoMatchSize(fieldData.NoMatchSize)
                .BoundaryMaxScan(fieldData.BoundaryMaxScan)
                .PhraseLimit(fieldData.PhraseLimit)
                .HighlightQuery(q => GetQuery(fieldData.Query))
                .BoundaryCharacters(fieldData.BoundaryChars)
                .BoundaryScanner((Nest.BoundaryScanner?)fieldData.BoundaryScanner)
                .BoundaryScannerLocale(fieldData.BoundaryScannerLocale)
                .FragmentOffset(fieldData.FragmentOffset)
                .MaxFragmentLength(fieldData.MaxFragmentLength)
                .Order((Nest.HighlighterOrder?)fieldData.Order)
                .PreTags(fieldData.PreTags ?? "")
                .PostTags(fieldData.PostTags ?? "")
                .RequireFieldMatch(fieldData.RequireFieldMatch)
                .TagsSchema((Nest.HighlighterTagsSchema?)fieldData.TagsSchema);

        #endregion

        #region MainQuery

        private static QueryContainer GetQuery<T>(QueryStruct<T> queryData) where T : class =>
            new QueryContainerDescriptor<T>()
                .Match(m => m
                    .Field(queryData?.Match?.Field)
                    .Query(queryData?.Match?.Text)
                    .Analyzer(queryData?.Match.Analyzer)
                    .AutoGenerateSynonymsPhraseQuery(queryData?.Match.AutoGenerateSynonymsPhraseQuery)
                    //.CutoffFrequency(queryData?.Match.CutoffFrequency) use it with [System.Obsolete]
                    .FuzzyTranspositions(queryData?.Match.FuzzyTranspositions)
                    .Lenient(queryData?.Match.Lenient)
                    .MaxExpansions(queryData?.Match.MaxExpansions)
                    .Operator((Nest.Operator?)(queryData?.Match.Operator))
                    .PrefixLength(queryData?.Match.PrefixLength)
                    .ZeroTermsQuery((Nest.ZeroTermsQuery?)(queryData?.Match.ZeroTermsQuery))
                );

        #endregion
    }
}

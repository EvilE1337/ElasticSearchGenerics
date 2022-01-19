using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FileSearch
{
    #region Suggest

    public class SuggestStruct<T>
    {
        public TermSuggest<T> Term { get; set; }
        public CompletionSuggest<T> Completion { get; set; }
        public PhraseSuggest<T> Phrase { get; set; }
    }

    public enum SuggestMode
    {
        Missing = 0,
        Popular = 1,
        Always = 2
    }

    #region Term

    public enum StringDistance
    {
        Internal = 0,
        DamerauLevenshtein = 1,
        Levenshtein = 2,
        Jarowinkler = 3,
        Ngram = 4
    }

    public enum SuggestSort
    {
        Score = 0,
        Frequency = 1
    }

    public class TermSuggest<T>
    {
        public string Name { get; set; }
        public bool? LowercaseTerms { get; set; }
        public int? MaxEdits { get; set; }
        public int? MaxInspections { get; set; }
        public int? MaxTermFrequency { get; set; }
        public int? MinDocFrequency { get; set; }
        public int? MinWordLength { get; set; }
        public int? PrefixLength { get; set; }
        public SuggestMode? SuggestMode { get; set; }
        public string Analyzer { get; set; }
        public Expression<Func<T, object>> Field { get; set; }
        public int? ShardSize { get; set; }
        public SuggestSort? Sort { get; set; }
        public StringDistance? StringDistance { get; set; }
        public int? Size { get; set; }
        public string Text { get; set; }
    }

    #endregion

    #region Completion

    public interface IFuzziness
    {
        bool Auto { get; }
        int? Low { get; }
        int? High { get; }
        int? EditDistance { get; }
        double? Ratio { get; }
    }

    public class CompletionFuzzy<T>
    {
        public IFuzziness Fuzziness { get; set; }
        public int MinLength { get; set; }
        public int PrefixLength { get; set; }
        public bool Transpositions { get; set; }
        public bool UnicodeAware { get; set; }
    }

    public class CompletionContext<T>
    {
        public string Name { get; set; }
        public string Context { get; set; }
        public double Boost { get; set; }
        public int? Precision { get; set; }
        public bool Prefix { get; set; }
    }

    public class CompletionSuggest<T>
    {
        public string Name { get; set; }
        public ICollection<CompletionContext<T>> Contexts { get; set; }
        public CompletionFuzzy<T> Fuzzy { get; set; }
        public string Prefix { get; set; }
        public string Regex { get; set; }
        public Expression<Func<T, object>> Field { get; set; }
        public bool? SkipDuplicates { get; set; }
        public int? Size { get; set; }
        public string Analyzer { get; set; }
    }

    #endregion

    #region Phrase

    public class PhraseCollateQuery
    {
        public string Id { get; set; }
        public string Source { get; set; }
    }

    public class PhraseCollate
    {
        public IDictionary<string, object> Params { get; set; }
        public bool? Prune { get; set; }
        public PhraseCollateQuery Query { get; set; }
    }

    public class PhraseDirectGenerator<T>
    {
        public Expression<Func<T, object>> Field { get; set; }
        public int? MaxEdits { get; set; }
        public float? MaxInspections { get; set; }
        public float? MaxTermFrequency { get; set; }
        public float? MinDocFrequency { get; set; }
        public int? MinWordLength { get; set; }
        public string PostFilter { get; set; }
        public string PreFilter { get; set; }
        public int? PrefixLength { get; set; }
        public int? Size { get; set; }
        public SuggestMode? SuggestMode { get; set; }
    }

    public class PhraseHighlight
    {
        public string PostTag { get; set; }
        public string PreTag { get; set; }
    }

    public class PhraseLaplaceSmoothing
    {
        public double? Alpha { get; set; }
    }

    public class PhraseLinearSmoothing
    {
        public double? BigramLambda { get; set; }
        public double? TrigramLambda { get; set; }
        public double? UnigramLambda { get; set; }
    }

    public class PhraseStupidBackoffSmoothing
    {
        public double? Discount { get; set; }
    }

    public class PhraseSmoothing
    {
        public PhraseLaplaceSmoothing Laplace { get; set; }
        public PhraseLinearSmoothing LinearInterpolation { get; set; }
        public PhraseStupidBackoffSmoothing StupidBackoff { get; set; }
    }

    public class PhraseSuggest<T>
    {
        public string Name { get; set; }
        public int? Size { get; set; }
        public string Analyzer { get; set; }
        public Expression<Func<T, object>> Field { get; set; }
        public string Text { get; set; }
        public PhraseCollate Collate { get; set; }
        public double? Confidence { get; set; }
        public PhraseDirectGenerator<T> DirectGenerator { get; set; }
        public bool? ForceUnigrams { get; set; }
        public int? GramSize { get; set; }
        public PhraseHighlight Highlight { get; set; }
        public double? MaxErrors { get; set; }
        public double? RealWordErrorLikelihood { get; set; }
        public char? Separator { get; set; }
        public int? ShardSize { get; set; }
        public PhraseSmoothing Smoothing { get; set; }
        public int? TokenLimit { get; set; }

    }

    #endregion

    #endregion

    #region Highlighter

    public enum HighlighterType
    {
        Plain = 0,
        Fvh = 1,
        Unified = 2
    }

    public enum HighlighterFragmenter
    {
        Simple = 0,
        Span = 1
    }

    public enum HighlighterEncoder
    {
        Default = 0,
        Html = 1
    }

    public enum BoundaryScanner
    {
        Characters = 0,
        Sentence = 1,
        Word = 2
    }

    public enum HighlighterOrder
    {
        Score = 0
    }

    public enum HighlighterTagsSchema
    {
        Styled = 0
    }

    public class HighlightFields<T>
    {
        public Expression<Func<T, object>> Field { get; set; }
        public HighlighterType? Type { get; set; }
        public bool? ForceSource { get; set; }
        public int? NumberOfFragments { get; set; }
        public int? NoMatchSize { get; set; }
        public string BoundaryChars { get; set; }
        public int? BoundaryMaxScan { get; set; }
        public BoundaryScanner? BoundaryScanner { get; set; }
        public string BoundaryScannerLocale { get; set; }
        public HighlighterFragmenter? Fragmenter { get; set; }
        public int? FragmentOffset { get; set; }
        public int? FragmentSize { get; set; }
        public int? MaxFragmentLength { get; set; }
        public int? PhraseLimit { get; set; }
        public QueryStruct<T> Query { get; set; }
        public HighlighterOrder? Order { get; set; }
        public string PreTags { get; set; }
        public string PostTags { get; set; }
        public bool? RequireFieldMatch { get; set; }
        public HighlighterTagsSchema? TagsSchema { get; set; }
    }

    public class HighlightStruct<T>
    {
        public string Tag { get; set; }
        public QueryStruct<T> HighlightQuery { get; set; }
        public HighlighterEncoder? Encoder { get; set; }
        public ICollection<HighlightFields<T>> Fields { get; set; }
        public string BoundaryChars { get; set; }
        public int? BoundaryMaxScan { get; set; }
        public BoundaryScanner? BoundaryScanner { get; set; }
        public string BoundaryScannerLocale { get; set; }
        public HighlighterFragmenter? Fragmenter { get; set; }
        public int? FragmentOffset { get; set; }
        public int? FragmentSize { get; set; }
        public int? MaxAnalyzedOffset { get; set; }
        public int? MaxFragmentLength { get; set; }
        public int? NoMatchSize { get; set; }
        public int? NumberOfFragments { get; set; }
        public HighlighterOrder? Order { get; set; }
        public bool? RequireFieldMatch { get; set; }
        public HighlighterTagsSchema? TagsSchema { get; set; }

    }

    #endregion

    #region MainQuery

    public enum ZeroTermsQuery
    {
        All = 0,
        None = 1
    }

    public enum Operator
    {
        And = 0,
        Or = 1
    }

    public class MatchQuery<T>
    {
        public Expression<Func<T, object>> Field { get; set; }
        public string Text { get; set; }
        public string Analyzer { get; set; }
        public bool? AutoGenerateSynonymsPhraseQuery { get; set; }
        public double? CutoffFrequency { get; set; }
        public bool? FuzzyTranspositions { get; set; }
        public bool? Lenient { get; set; }
        public int? MaxExpansions { get; set; }
        public Operator? Operator { get; set; }
        public int? PrefixLength { get; set; }
        public ZeroTermsQuery? ZeroTermsQuery { get; set; }
    }

    public class QueryStruct<T>
    {
        public MatchQuery<T> Match { get; set; }
    }

    #endregion

    #region Aggregations

    public class AggregationsStruct<T>
    {

    }

    #endregion

    public partial class ElasticSearchArguments<T>
    {
        public QueryStruct<T> Query { get; set; }
        public AggregationsStruct<T> Aggregations { get; set; }
        public HighlightStruct<T> Highlight { get; set; }
        public SuggestStruct<T> Suggest { get; set; }
    }
}

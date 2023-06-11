using System.Globalization;

namespace LanguageModel;

/// <summary>
/// Represents an ngram based & already trained language model.
/// </summary>
public class NGramLanguageModel
{
    /// <summary>
    /// All ngrams that are contained in this language model.
    /// </summary>
    public readonly IDictionary<uint,NGram> NGrams;

    /// <summary>
    /// Initializes a new instance of <see cref="NGramLanguageModel"/>.
    /// </summary>
    public NGramLanguageModel()
    {
        NGrams = new Dictionary<uint,NGram>();
    }

    /// <summary>
    /// Adds a new ngram to this language model.
    /// </summary>
    /// <param name="size">The size of the ngram.</param>
    /// <param name="context">The context of the ngram.</param>
    /// <param name="next">The next word of the ngram.</param>
    /// <param name="possibility">The probability of the ngram.</param>
    public void AddNGram(uint size, string context, string next, double possibility)
    {
        if (!NGrams.TryGetValue(size, out NGram? ngram))
        {
            ngram = new NGram(size);
            NGrams.Add(size, ngram);
        }
        ngram.AddNGram(context, next, possibility);
    }

    /// <summary>
    /// Creates an ARPA-formatted representation of the language model that is modeled by this instance and writes it using the provided streamwriter.
    /// </summary>
    /// <param name="outputStreamWriter">The output streamwriter used to write the ARPA representation of the language model.</param>
    public void GetArpaRepresentation(StreamWriter outputStreamWriter)
    {
        // Write header part
        outputStreamWriter.WriteLine("\\data\\");
        foreach (var item in NGrams.Values)
        {
            int count = 0;
            foreach (var ngram in item.NGrams.Values)
            {
                count += ngram.Count;
            }
            outputStreamWriter.WriteLine($"ngram {item.Size} = {count}");
        }

        // Write body with ngrams and their probabilities
        outputStreamWriter.WriteLine();
        foreach (var item in NGrams.Values)
        {
            item.GetArpaRepresentation(outputStreamWriter);
            outputStreamWriter.WriteLine();
        }

        // Write end marker of ARPA format
        outputStreamWriter.WriteLine("\\end\\");
        outputStreamWriter.Flush();
    }

    /// <summary>
    /// Creates an ARPA-formatted representation of the language model that is modeled by this instance and writes it to the provided output stream.
    /// </summary>
    /// <param name="outputStream">The output stream that the ARPA representation of the language model will be written to.</param>
    public void GetArpaRepresentation(Stream outputStream) => GetArpaRepresentation(new StreamWriter(outputStream));

    /// <summary>
    /// Creates a new instance of <see cref="NGramLanguageModel"/> that represents the
    /// ARPA-formatted language model whose data is read using the provided stream reader.
    /// </summary>
    /// <param name="inputStream">The stream reader that will be used to read the ARPA-formatted language model data.</param>
    /// <returns>A new instance od <see cref="NGramLanguageModel"/> that represents the read language model.</returns>
    /// <exception cref="InvalidDataException">The provided ARPA-formatted data is not valid.</exception>
    public static NGramLanguageModel LoadFrom(StreamReader inputStream)
    {
        // First line of header
        string? currentLine = inputStream.ReadLine();
        string expected = "\\data\\";
        if (!expected.Equals(currentLine))
        {
            throw new InvalidDataException($"Invalid ARPA data: Expected \"{expected}\", but got {currentLine}");
        }

        // Remaining part of header, including empty line separating it from ngram data
        var counts = new Dictionary<int, int>();
        while((currentLine = inputStream.ReadLine()) != "")
        {
            if (currentLine is null)
            {
                throw new InvalidDataException($"Invalid ARPA data: Expected \"ngram <x> = <y>\", but got NULL");
            }
            string[] splitted = currentLine.Split(' ');
            if (splitted.Length != 4)
            {
                throw new InvalidDataException($"Invalid ARPA data: Expected \"ngram <x> = <y>\", but got {currentLine}");
            }

            // Parse one header line and check it
            bool success = true;
            success &= splitted[0].Equals("ngram");
            success &= splitted[2].Equals("=");
            success &= int.TryParse(splitted[1], CultureInfo.InvariantCulture, out int ngramSize);
            success &= int.TryParse(splitted[3], CultureInfo.InvariantCulture, out int ngramCount);
            if (!success)
            {
                throw new InvalidDataException($"Invalid ARPA data: Expected \"ngram <x> = <y>\" with <x> and <y> being positive integers, but got {currentLine}");
            }
            if (counts.ContainsKey(ngramSize))
            {
                throw new InvalidDataException($"Invalid ARPA data: Multiple count entries for ngram size {ngramSize}");
            }

            // save the values to perform a consistency check later on
            counts.Add(ngramSize, ngramCount);
        }

        var lm = new NGramLanguageModel();

        // NGram probabilities
        for (int size = 1; size <= counts.Count; size++)
        {
            // NGram probabilities - header saying size of following ngrams
            currentLine = inputStream.ReadLine();
            expected = $"\\{size}-grams:";
            if (!expected.Equals(currentLine))
            {
                throw new InvalidDataException($"Invalid ARPA data: Expected \"{expected}\", but got \"{currentLine}\"");
            }

            // NGram probabilities - the real values
            for (int count = 0; count < counts[size]; count++)
            {
                currentLine = inputStream.ReadLine();
                if (currentLine is null || currentLine.Equals(""))
                {
                    throw new InvalidDataException($"Invalid ARPA data: Expected \"ngram <x> = <y>\", but got NULL");
                }

                // Parse single ngram data
                string[] splitted = currentLine.Split(' ');
                if (splitted.Length != size + 1)
                {
                    throw new InvalidDataException($"Invalid ARPA data: Expected \"<probability> <ngram>\", but got {currentLine} - wrong ngram format");
                }
                if (!double.TryParse(splitted[0], CultureInfo.InvariantCulture, out double pLog))
                {
                    throw new InvalidDataException($"Invalid ARPA data: Expected \"<probability> <ngram>\" with probability being a double, but got {currentLine}");
                }

                // Calculate probability
                double p = Math.Pow(10, pLog);
                if (p > 1)
                {
                    Console.WriteLine($"WARN: Encountered a probability > 1: {p}");
                }

                // Get next and context of ngram
                string next = splitted.Last();
                string context = string.Join(' ', splitted.Take(new Range(new Index(1), new Index(1, true))));

                // Save to language model that is being build from ARPA-formatted serialized data
                lm.AddNGram((uint)size, context, next, p);
            }

            // NGram probabilities - blank line after all ngrams of one size
            // This will recognize if the actual ngram counts and these in the header match.
            // If not this will throw an exception.
            currentLine = inputStream.ReadLine();
            expected = "";
            if (!expected.Equals(currentLine))
            {
                throw new InvalidDataException($"Invalid ARPA data: Expected empty line, but got \"{currentLine}\". Probably the header is not consistent with the data found in the body.");
            }
        }

        // Footer
        currentLine = inputStream.ReadLine();
        expected = "\\end\\";
        if (!expected.Equals(currentLine))
        {
            throw new InvalidDataException($"Invalid ARPA data: Expected \"{expected}\", but got \"{currentLine}\"");
        }

        return lm;
    }

    /// <summary>
    /// Creates a new instance of <see cref="NGramLanguageModel"/> that represents the
    /// ARPA-formatted language model whose data is read using the provided stream reader.
    /// </summary>
    /// <param name="inputStream">The stream that the ARPA-formatted language model data will be read from.</param>
    /// <returns>A new instance od <see cref="NGramLanguageModel"/> that represents the read language model.</returns>
    /// <exception cref="InvalidDataException">The provided ARPA-formatted data is not valid.</exception>
    public static NGramLanguageModel LoadFrom(string filePath) => LoadFrom(File.OpenText(filePath));

    /// <summary>
    /// Computes the perplexity of a given input sentence using the language model data that is represented by this instance.
    /// </summary>
    /// <param name="sentence">The sentence to calculate the preplexity of.</param>
    /// <returns>The perplexity of the sentence.</returns>
    public double GetPerplexity(string sentence)
    {
        List<string> tokens = sentence.Split(' ').ToList();
        tokens.Insert(0, "<s>");
        tokens.Add("</s>");
        double sentenceProbability = 1;
        uint size = NGrams.Keys.Max();

        // Calculate perplexity by splitting into ngrams, checking for match in language model and multiplying the results according to formula in slides
        for (int index = 0; index < tokens.Count; index++)
        {
            // Get probability of ngram
            double p = 0;
            string next = tokens[index];
            uint currentSearchedSize = size;
            do
            {
                // Index of first word we have to consider as context for the current searched ngram length
                int contextStartIndex = (int)(index - currentSearchedSize + 1);

                // Check if we actually got enough words for context with current searched ngram length,
                // i.e. we can't cover the first word of the sentence with a 3-gram
                if (contextStartIndex < 0) 
                {
                    currentSearchedSize--;
                    continue;
                }

                // Get context, first index is included, last is not, so we don't include the next word in context here
                string context = string.Join(' ', tokens.Take(new Range(new Index(contextStartIndex), new Index((int)(index)))));

                // Check if we got a probability value for this ngram, if we have so we store it in p and the loop finishes as then p != 0
                if (NGrams[currentSearchedSize].NGrams.ContainsKey(context) && NGrams[currentSearchedSize].NGrams[context].ContainsKey(next))
                {
                    p = NGrams[currentSearchedSize].NGrams[context][next];
                }
                else
                {
                    // No probability for this ngram in our database -> we have to consider shorter ngrams or abort if we already checked 1-grams
                    currentSearchedSize--;
                }
            } while (currentSearchedSize > 0 && p == 0); // check all ngram sizes starting from longest until we checked all or got a value

            // Multiply to result
            // We could also do this in log space, but a double type has a lot of precision in C# (minimum value is around 5.0 * 10^-324)
            sentenceProbability *= p;
        }

        // Calculate cross-entropy according to formula in slides
        double crossEntropy = (double)-1 / tokens.Count * Math.Log10(sentenceProbability);

        // Calculate perplexity according to formula in slides
        double perplexity = Math.Pow(2, crossEntropy);

        return perplexity;
    }
}

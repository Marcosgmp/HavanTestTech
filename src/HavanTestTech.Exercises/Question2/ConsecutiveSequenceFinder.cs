namespace HavanTestTech.Exercises.Question2;

public static class ConsecutiveSequenceFinder{

    // <summary>
    //  Returns the longest run of consecutive integers in the input, regardless of element order.
    //  Runs in O(n): a hash set replaces sorting, which would cost O(n log n).
    //  </summary>
    public static IReadOnlyList<int> FindLongest(IEnumerable<int> numbers)
    {
        ArgumentNullException.ThrowIfNull(numbers);

        // The set discards duplicates, which must not lengthen a run.
        var numberSet = new HashSet<int>(numbers);
        var bestStart = 0;
        var bestLength = 0;

        foreach (var number in numberSet)
        {
            // A number that has a predecessor sits in the middle of a run. Expanding only
            // from run starts keeps the total work linear.
            if (numberSet.Contains(number - 1))
            {
                continue;
            }

            var length = MeasureRun(numberSet, number);
            if (length > bestLength)
            {
                bestStart = number;
                bestLength = length;
            }
        }

        return Enumerable.Range(bestStart, bestLength).ToList();
    }

    private static int MeasureRun(HashSet<int> numberSet, int start)
    {
        var length = 1;
        while (numberSet.Contains(start + length))
        {
            length++;
        }

        return length;
    }
}
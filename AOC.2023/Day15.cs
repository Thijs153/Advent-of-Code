namespace AOC._2023;

public class Day15
{
    private readonly string _input = File.ReadAllText("Inputs/Day15.txt");

    [Fact]
    public void Part1() => _input.Split(',').Select(Hash).Sum().Should().Be(509152);

    [Fact]
    public void Part2() => ParseSteps(_input).Aggregate(MakeBoxes(256), UpdateBoxes, GetFocusingPower).Should().Be(244403);
    
    private static int Hash(string st) => st.Aggregate(0, (ch, a) => (ch + a) * 17 % 256);

    private static List<Lens>[] UpdateBoxes(List<Lens>[] boxes, Step step)
    {
        var box = boxes[Hash(step.Label)];
        var iLens = box.FindIndex(lens => lens.Label == step.Label);

        if (!step.FocalLength.HasValue && iLens >= 0)
        {
            box.RemoveAt(iLens);
        }
        else if (step.FocalLength.HasValue && iLens >= 0)
        {
            box[iLens] = new Lens(step.Label, step.FocalLength.Value);
        }
        else if (step.FocalLength.HasValue && iLens < 0)
        {
            box.Add(new Lens(step.Label, step.FocalLength.Value));
        }

        return boxes;
    }

    private static IEnumerable<Step> ParseSteps(string input) =>
        from item in input.Split(',')
        let parts = item.Split('-', '=')
        select new Step(parts[0], parts[1] == "" ? null : int.Parse(parts[1]));

    private static List<Lens>[] MakeBoxes(int count) =>
        Enumerable.Range(0, count).Select(_ => new List<Lens>()).ToArray();

    private static int GetFocusingPower(List<Lens>[] boxes) => (
        from iBox in Enumerable.Range(0, boxes.Length)
        from iLens in Enumerable.Range(0, boxes[iBox].Count)
        select (iBox + 1) * (iLens + 1) * boxes[iBox][iLens].FocalLength
    ).Sum();
    
    private record Lens(string Label, int FocalLength);
    
    private record Step(string Label, int? FocalLength);
}



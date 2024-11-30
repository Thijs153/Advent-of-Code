namespace AOC._2021;

public class Day19
{
    private readonly string _input = File.ReadAllText("Inputs/Day19.txt").Replace("\r", "");

    [Fact]
    public void Part1()
    {
        LocateScanners(_input)
            .SelectMany(scanner => scanner.GetBeaconsInWorld())
            .Distinct()
            .Count()
            .Should().Be(392);
    }

    [Fact]
    public void Part2()
    {
        var scanners = LocateScanners(_input);
        (
            from sA in scanners
            from sB in scanners
            where sA != sB
            select
                Math.Abs(sA.center.x - sB.center.x) +
                Math.Abs(sA.center.y - sB.center.y) +
                Math.Abs(sA.center.z - sB.center.z)
        ).Max()
        .Should().Be(13332);
    }

    private static HashSet<Scanner> LocateScanners(string input)
    {
        var scanners = new HashSet<Scanner>(Parse(input));
        var locatedScanners = new HashSet<Scanner>();
        var q = new Queue<Scanner>();
        
        // when a scanner is located, it gets into the queue so that we can
        // explore its neighbours.

        locatedScanners.Add(scanners.First());
        q.Enqueue(scanners.First());

        scanners.Remove(scanners.First());

        while (q.Any())
        {
            var scannerA = q.Dequeue();
            foreach (var scannerB in scanners.ToArray())
            {
                var maybeLocatedScanner = TryToLocate(scannerA, scannerB);
                if (maybeLocatedScanner != null)
                {
                    locatedScanners.Add(maybeLocatedScanner);
                    q.Enqueue(maybeLocatedScanner);

                    scanners.Remove(scannerB);
                }
            }
        }

        return locatedScanners;
    }

    private static Scanner TryToLocate(Scanner scannerA, Scanner scannerB)
    {
        var beaconsInA = scannerA.GetBeaconsInWorld().ToArray();

        foreach (var (beaconInA, beaconInB) in PotentialMatchingBeacons(scannerA, scannerB))
        {
            // now try to find the orientation for B:
            var rotatedB = scannerB;
            for (var rotation = 0; rotation < 24; rotation++, rotatedB = rotatedB.Rotate())
            {
                // Moving the rotated scanner so that beaconA and beaconB overlaps. Are there 12 matches?
                var beaconInRotatedB = rotatedB.Transform(beaconInB);

                var locatedB = rotatedB.Translate(new Coord(
                    beaconInA.x - beaconInRotatedB.x,
                    beaconInA.y - beaconInRotatedB.y,
                    beaconInA.z - beaconInRotatedB.z
                ));

                if (locatedB.GetBeaconsInWorld().Intersect(beaconsInA).Count() >= 12)
                    return locatedB;
            }
        }
        
        // no luck
        return null;
    }

    private static IEnumerable<(Coord beaconInA, Coord beaconInB)> PotentialMatchingBeacons(Scanner scannerA, Scanner scannerB)
    {
        /*
         * If we had a matching beaconInA and beaconInB and moved the center
         * of the scanners to these then we would find at least 12 beacons
         * with the same coordinates
         *
         * The only problem is that the rotation of scannerB is not fixed yet.
         *
         * We need to make our check invariant to that:
         *
         * After the translation, we could form a set from each scanner
         * taking the absolute values of the x y and z coordinates of their beacons
         * and compare those.
         */

        IEnumerable<int> AbsCoordinates(Scanner scanner) =>
            from coord in scanner.GetBeaconsInWorld()
            from v in new[] { coord.x, coord.y, coord.z }
            select Math.Abs(v);
        
        /*
         * This is the same no matter how we rotate scannerB, so the two sets should
         * have at least  3 * 12 common values (with multiplicity)
         *
         * We can also considerably speed up the search with the pigeonhole principle
         * which says that it's enough to take all but 11 beacons from A and B.
         * If there is no match amongst those, there cannot be 12 matching pairs:
         */

        IEnumerable<T> Pick<T>(IEnumerable<T> ts) => ts.Take(ts.Count() - 11);

        foreach (var beaconInA in Pick(scannerA.GetBeaconsInWorld()))
        {
            var absA = AbsCoordinates(
                scannerA.Translate(new Coord(-beaconInA.x, -beaconInA.y, -beaconInA.z))
            ).ToHashSet();

            foreach (var beaconInB in Pick(scannerB.GetBeaconsInWorld()))
            {
                var absB = AbsCoordinates(
                    scannerB.Translate(new Coord(-beaconInB.x, -beaconInB.y, -beaconInB.z))
                );

                if (absB.Count(d => absA.Contains(d)) >= 3 * 12)
                {
                    yield return (beaconInA, beaconInB);
                }
            }
        }
    }

    private static  Scanner[] Parse(string input) => (
        from block in input.Split("\n\n")
        let beacons =
            from line in block.Split("\n").Skip(1)
            let parts = line.Split(",").Select(int.Parse).ToArray()
            select new Coord(parts[0], parts[1], parts[2])
        select new Scanner(new Coord(0, 0, 0), 0, beacons.ToList())
    ).ToArray();
    
    private record Coord(int x, int y, int z);

    private record Scanner(Coord center, int rotation, List<Coord> beaconsInLocal)
    {
        public Scanner Rotate() => this with { rotation = rotation + 1 };
        public Scanner Translate(Coord t) => this with { center = new Coord(center.x + t.x, center.y + t.y, center.z + t.z) };

        public Coord Transform(Coord coord)
        {
            var (x, y, z) = coord;
            
            // rotate coordinate system so that x-axis points in the possible 6 directions
            (x, y, z) = (rotation % 6) switch
            {
                0 => (x, y, z),
                1 => (-x, y, -z),
                2 => (y, -x, z),
                3 => (-y, x, z),
                4 => (z, y, -x),
                5 => (-z, y, x),
                _ => (x, y, z)
            };

            // rotate around x-axis:
            (x, y, z) = ((rotation / 6) % 4) switch
            {
                0 => (x, y, z),
                1 => (x, -z, y),
                2 => (x, -y, -z),
                3 => (x, z, -y),
                _ => (x, y, z)
            };

            return new Coord(center.x + x, center.y + y, center.z + z);
        }

        public IEnumerable<Coord> GetBeaconsInWorld() => beaconsInLocal.Select(Transform);
    };

}
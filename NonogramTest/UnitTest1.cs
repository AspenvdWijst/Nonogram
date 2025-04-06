using Xunit;
using System.Reflection;
using Nonogram;

namespace NonogramTest
{
    public class Form1Tests
    {
        private Form1 form;

        public Form1Tests()
        {
            int gridSize = 0;
            form = new Form1(gridSize);

            // Inject a known solution grid for predictable results
            typeof(Form1).GetField("GridSize", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(form, 5);
            bool[,] grid = new bool[5, 5]
            {
                { true, true, false, true, false },
                { false, false, false, false, false },
                { true, true, true, true, true },
                { false, true, false, true, false },
                { true, false, true, false, true }
            };

            typeof(Form1).GetField("solutionGrid", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(form, grid);
        }

        [Fact]
        public void GetRowClue_ReturnsCorrectClue_ForMixedRow()
        {
            MethodInfo getRowClue = typeof(Form1).GetMethod("GetRowClue", BindingFlags.NonPublic | BindingFlags.Instance);
            string clue = (string)getRowClue.Invoke(form, new object[] { 0 });

            Assert.Equal("2 1", clue); // Row 0: two filled, then one
        }

        [Fact]
        public void GetColumnClue_ReturnsCorrectClue_ForAlternatingColumn()
        {
            MethodInfo getColumnClue = typeof(Form1).GetMethod("GetColumnClue", BindingFlags.NonPublic | BindingFlags.Instance);
            string clue = (string)getColumnClue.Invoke(form, new object[] { 0 });

            Assert.Equal("1 1 1", clue); // Column 0: filled on row 0, 2, 4
        }
    }
}

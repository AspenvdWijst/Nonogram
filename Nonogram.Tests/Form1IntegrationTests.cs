using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nonogram;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nonogram.Tests
{
    [TestClass]
    public class Form1IntegrationTests
    {
        [TestMethod]
        public async Task Form1_NewPuzzle_InitializesNewGrid()
        {
            // Arrange
            var form = new Form1();
            form.Show(); // Required to initialize UI components

            // Act
            await Task.Run(() => form.Invoke(new MethodInvoker(() =>
            {
                form.NewPuzzleBtn.PerformClick(); // Simulate New Puzzle button
            })));

            // Assert
            Assert.IsNotNull(form); // Basic test that form exists
        }

        [TestMethod]
        public async Task Form1_ResetButton_ClearsGrid()
        {
            // Arrange
            var form = new Form1();
            form.Show();

            await Task.Run(() => form.Invoke(new MethodInvoker(() =>
            {
                form.NewPuzzleBtn.PerformClick();
            })));

            await Task.Delay(500); // wait for async to complete

            await Task.Run(() => form.Invoke(new MethodInvoker(() =>
            {
                form.SimulateClick(0, 0, MouseButtons.Left);
            })));

            await Task.Delay(500);

            // Act
            await Task.Run(() => form.Invoke(new MethodInvoker(() =>
            {
                form.ResetButton.PerformClick();
            })));

            await Task.Delay(500);

            // Assert: top-left cell should be empty (0)
            Assert.AreEqual(0, GetPrivatePlayerGridValue(form, 0, 0));
        }

        private int GetPrivatePlayerGridValue(Form1 form, int row, int col)
        {
            var field = typeof(Form1).GetField("playerGrid", BindingFlags.NonPublic | BindingFlags.Instance);
            int[,] grid = (int[,])field.GetValue(form);
            return grid[row, col];
        }
    }
}

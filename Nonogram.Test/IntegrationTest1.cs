using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows.Forms;
using Nonogram;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.IO.Packaging;

namespace NonogramTests
{
    [TestClass]
    public class NonogramIntegrationTests
    {
        private Form1 form;

        // Set up the form for the test
        [TestInitialize]
        public void Setup()
        {
            // Run form in a new thread for UI testing
            var thread = new Thread(() =>
            {
                form = new Form1();
                Application.Run(form);
            });
            thread.SetApartmentState(ApartmentState.STA); // Required for UI
            thread.Start();

            Thread.Sleep(2000); // Wait for the form to load
        }

        // Test method om newpuzzlebutton te testen WERKT ALLEEN ZONDER TIMER

        [TestMethod]
        public async Task Test_NewPuzzle_ResetsPlayerGrid()
        {
            form.Invoke((MethodInvoker)(async () =>
            {
                form.NewPuzzleBtn_Click(null, EventArgs.Empty);

                // Wait a bit for the UI to process the reset
                await Task.Delay(500);

                bool allEmpty = true;
                for (int i = 0; i < 5; i++) // Default 5x5
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (GetPrivateGrid(form)[i, j] != 0)
                        {
                            allEmpty = false;
                            break;
                        }
                    }
                }
                Assert.IsTrue(allEmpty, "New puzzle should reset the grid to all empty.");
            }));
        }

        // Test if a cell click updates the grid correctly
        [TestMethod]
        public void Test_GridCellClick_UpdatesGrid()
        {
            form.Invoke((MethodInvoker)(async () =>
            {
                // Simulate clicking on the cell at (1,1)
                int gridStart = 100;
                int cellSize = 100;
                int x = gridStart + 1 * cellSize + cellSize / 2;
                int y = gridStart + 1 * cellSize + cellSize / 2;

                MouseEventArgs click = new MouseEventArgs(MouseButtons.Left, 1, x, y, 0);
                form.OnMouseClick(null, click);  // Trigger the click event

                await Task.Delay(100);  // Wait for the UI to update
                Assert.AreEqual(1, GetPrivateGrid(form)[1, 1], "Cell (1,1) should be marked as filled.");
            }));
        }

        // Test if saving the grid creates the file
        [TestMethod]
        public async Task Test_SaveGrid_CreatesFile()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\SavedGrid.json");

            form.Invoke((MethodInvoker)(async () =>
            {
                // Simulate a click to modify the grid
                int gridStart = 100;
                int cellSize = 100;
                MouseEventArgs click = new MouseEventArgs(MouseButtons.Left, 1, gridStart + 50, gridStart + 50, 0);
                form.OnMouseClick(null, click);

                await Task.Delay(100);  // Wait for the UI to process the change

                // Check if the file is created and contains the right data
                Assert.IsTrue(File.Exists(path), "SavedGrid.json should exist after a change.");
                string content = await File.ReadAllTextAsync(path);
                Assert.IsTrue(content.Contains("1"), "File content should include '1' after marking a cell.");
            }));
        }

        // Clean up after the test
        [TestCleanup]
        public void Cleanup()
        {
            form.Invoke((MethodInvoker)(() => form.Close()));  // Close the form after the test
        }

        // Get the private playerGrid using reflection
        private int[,] GetPrivateGrid(Form1 form)
        {
            var field = typeof(Form1).GetField("playerGrid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (int[,])field.GetValue(form);  // Return the grid
        }
    }
}

using System;
using System.Windows.Forms;

namespace Property_Tax
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            for (int i = 1; i <= 14; i++)
            {
                var ssvTextBox = this.Controls.Find($"additionssv{i}", true).FirstOrDefault() as TextBox;
                if (ssvTextBox != null)
                {
                    ssvTextBox.TextChanged += UpdateSSVTotal;
                }
            }

            for (int i = 1; i <= 14; i++)  // Assuming you have 14 rows
            {
                var lvTextBox = this.Controls.Find($"assessmentrecordlv{i}", true).FirstOrDefault() as TextBox;
                var bvTextBox = this.Controls.Find($"assessmentrecordbv{i}", true).FirstOrDefault() as TextBox;

                if (lvTextBox != null && bvTextBox != null)
                {
                    lvTextBox.Tag = i;
                    bvTextBox.Tag = i;

                    lvTextBox.TextChanged += UpdateTotal;
                    bvTextBox.TextChanged += UpdateTotal;
                }
            }

            for (int i = 1; i <= 3; i++)  // Loop for each row
            {
                for (int j = 1; j <= 3; j++) // Loop for each exemption ComboBox in the current row
                {
                    var exemptComboBox = this.Controls.Find($"assessmentrecordexempt{j}_{i}", true).FirstOrDefault() as ComboBox;
                    if (exemptComboBox != null)
                    {
                        exemptComboBox.Tag = i;
                        exemptComboBox.SelectedIndexChanged += UpdateTotal;
                    }
                }
            }

            for (int i = 1; i <= 14; i++)  // Assuming you have 14 rows
            {
                var porchComboBox = this.Controls.Find($"additionsporchtype{i}", true).FirstOrDefault() as ComboBox;
                var plumbingComboBox = this.Controls.Find($"additionsplumbingtype{i}", true).FirstOrDefault() as ComboBox;

                if (porchComboBox != null)
                {
                    porchComboBox.Tag = i;
                    porchComboBox.SelectedValueChanged += CalculatePrice;
                }

                if (plumbingComboBox != null)
                {
                    plumbingComboBox.Tag = i;
                    plumbingComboBox.SelectedValueChanged += CalculatePrice;
                }
            }

            for (int i = 1; i <= 14; i++)  // Assuming you have 14 rows
            {
                var typeComboBox = this.Controls.Find($"additiontype{i}", true).FirstOrDefault() as ComboBox;
                var ssfTextBox = this.Controls.Find($"additionssf{i}", true).FirstOrDefault() as TextBox;

                if (typeComboBox != null && ssfTextBox != null)
                {
                    typeComboBox.Tag = i;  // Set the row number as the tag
                    ssfTextBox.Tag = i;

                    typeComboBox.SelectedIndexChanged += CalculatePrice;
                    ssfTextBox.TextChanged += CalculatePrice;
                }
            }
        }

        private void UpdateSSVTotal(object sender, EventArgs e)
        {
            double total = 0;

            for (int i = 1; i <= 14; i++)
            {
                var ssvTextBox = this.Controls.Find($"additionssv{i}", true).FirstOrDefault() as TextBox;
                if (ssvTextBox != null && double.TryParse(ssvTextBox.Text, out double value))
                {
                    total += value;
                }
            }

            var ssvTotalTextBox = this.Controls.Find("additionssvtotal", true).FirstOrDefault() as TextBox;
            if (ssvTotalTextBox != null)
            {
                ssvTotalTextBox.Text = total.ToString("N2");
            }
        }

        private void UpdateTotal(object sender, EventArgs e)
        {
            if (sender is Control control && control.Tag is int rowNumber)
            {
                var lvTextBox = this.Controls.Find($"assessmentrecordlv{rowNumber}", true).FirstOrDefault() as TextBox;
                var bvTextBox = this.Controls.Find($"assessmentrecordbv{rowNumber}", true).FirstOrDefault() as TextBox;
                var totalTextBox = this.Controls.Find($"assessmentrecordtotal{rowNumber}", true).FirstOrDefault() as TextBox;

                if (lvTextBox != null && bvTextBox != null && totalTextBox != null)
                {
                    double.TryParse(lvTextBox.Text, out double landValue);
                    double.TryParse(bvTextBox.Text, out double buildingValue);

                    // Initialize total with land and building values
                    double total = landValue + buildingValue;

                    for (int i = 1; i <= 3; i++)
                    {
                        var exemptComboBox = this.Controls.Find($"assessmentrecordexempt{i}_{rowNumber}", true).FirstOrDefault() as ComboBox;
                        if (exemptComboBox?.SelectedItem != null && exemptionValues.ContainsKey(exemptComboBox.SelectedItem.ToString()))
                        {
                            total -= exemptionValues[exemptComboBox.SelectedItem.ToString()];
                        }
                    }

                    // Update the total box
                    totalTextBox.Text = total.ToString("N2"); // Format as currency or however you prefer
                }
            }
        }

        private Dictionary<string, double> exemptionValues = new Dictionary<string, double>
            {
                { "Veteran", 6000.0 },
                { "Veteran (Non-Maine Enlisted)", 6000.0 },
                { "Homestead", 25000.0 },
                // ... add other exemption types and values
            };

        private Dictionary<string, int> plumbingData = new Dictionary<string, int>
        {
            {"No Plumbing", -3000},
            {"Toilet Room - No Bathroom", -600},
            {"Additional Baths", 1800},
            {"Toilet Room", 1200},
            {"Water Closet", 600},
            {"Lavatory", 600},
            {"Stall Shower", 600},
            {"Hot Tub", 1200},
            {"Sauna", 1200},
            {"Bathroom", 0},
            {"Kitchen", 0},
            {"Additional Kitchen", 0}
        };

        private Dictionary<string, Dictionary<int, int>> porchpriceData = new Dictionary<string, Dictionary<int, int>>
        {
            {"OP1St.Fl", new Dictionary<int, int>
                {
                    {50, 6}, {75, 8}, {100, 11}, {125, 13}, {150, 14}, {175, 16}, {200, 17}, {225, 18}, {250, 19},
                    {275, 21}, {300, 23}, {325, 24}, {350, 26}, {375, 28}, {400, 30}, {450, 34}, {500, 38}, {550, 41},
                    {600, 45}, {650, 49}, {700, 53}, {750, 56}, {800, 60}, {850, 64}, {900, 68}, {950, 71}, {1000, 75}
                }
            },
            {"OP2ndFl", new Dictionary<int, int>
                {
                    {50, 3}, {75, 4}, {100, 6}, {125, 7}, {150, 7}, {175, 8}, {200, 9}, {225, 9}, {250, 10},
                    {275, 11}, {300, 12}, {325, 12}, {350, 13}, {375, 14}, {400, 15}, {450, 17}, {500, 19}, {550, 21},
                    {600, 23}, {650, 24}, {700, 26}, {750, 28}, {800, 30}, {850, 32}, {900, 34}, {950, 36}, {1000, 38}
                }
            },
            {"Fin.E.P1St.Fl", new Dictionary<int, int>
                {
                    {50, 12}, {75, 16}, {100, 22}, {125, 26}, {150, 28}, {175, 32}, {200, 34}, {225, 36}, {250, 38},
                    {275, 42}, {300, 46}, {325, 49}, {350, 53}, {375, 56}, {400, 60}, {450, 68}, {500, 75}, {550, 83},
                    {600, 90}, {650, 98}, {700, 105}, {750, 113}, {800, 120}, {850, 128}, {900, 135}, {950, 143}, {1000, 150}
                }
            },
            {"Unfin.E.P1St.Fl", new Dictionary<int, int>
                {
                    {50, 9}, {75, 12}, {100, 17}, {125, 20}, {150, 21}, {175, 24}, {200, 26}, {225, 27}, {250, 29},
                    {275, 32}, {300, 35}, {325, 37}, {350, 39}, {375, 42}, {400, 45}, {450, 51}, {500, 56}, {550, 62},
                    {600, 68}, {650, 73}, {700, 79}, {750, 84}, {800, 90}, {850, 96}, {900, 101}, {950, 107}, {1000, 113}
                }
            },
            {"Fin.E.P2nd.Fl", new Dictionary<int, int>
                {
                    {50, 6}, {75, 8}, {100, 11}, {125, 13}, {150, 14}, {175, 16}, {200, 17}, {225, 18}, {250, 19},
                    {275, 21}, {300, 23}, {325, 24}, {350, 26}, {375, 28}, {400, 30}, {450, 34}, {500, 38}, {550, 41},
                    {600, 45}, {650, 49}, {700, 53}, {750, 56}, {800, 60}, {850, 64}, {900, 68}, {950, 71}, {1000, 75}
                }
            },
            {"Unfin.E.P2nd.Fl", new Dictionary<int, int>
                {
                    {50, 5}, {75, 6}, {100, 8}, {125, 10}, {150, 11}, {175, 12}, {200, 13}, {225, 14}, {250, 14},
                    {275, 16}, {300, 17}, {325, 18}, {350, 20}, {375, 21}, {400, 23}, {450, 25}, {500, 28}, {550, 31},
                    {600, 34}, {650, 37}, {700, 39}, {750, 42}, {800, 45}, {850, 48}, {900, 51}, {950, 53}, {1000, 56}
                }
            },
            {"Bsmt.Entry", new Dictionary<int, int>
                {
                    {50, 9}, {75, 12}, {100, 17}, {125, 20}, {150, 21}, {175, 24}, {200, 26}, {225, 27}, {250, 29},
                    {275, 32}, {300, 35}, {325, 37}, {350, 39}, {375, 42}, {400, 45}, {450, 51}, {500, 56}, {550, 62},
                    {600, 68}, {650, 73}, {700, 79}, {750, 84}, {800, 90}, {850, 96}, {900, 101}, {950, 107}, {1000, 113}
                }
            }
         };

        private Dictionary<string, Dictionary<int, int>> priceData = new Dictionary<string, Dictionary<int, int>>
        {
            {"1St", new Dictionary<int, int>
                {
                    {50, 31}, {65, 37}, {75, 41}, {85, 47}, {100, 51}, {125, 55}, {150, 59}, {175, 64}, {200, 69},
                    {225, 73}, {250, 78}, {275, 83}, {300, 88}, {350, 99}, {400, 110}, {450, 121}, {500, 132},
                    {550, 143}, {600, 154}, {650, 163}, {700, 176}, {750, 187}, {800, 198}, {850, 209}, {900, 220}
                }
            },
            {"1St/B", new Dictionary<int, int>
                {
                    {50, 37}, {65, 44}, {75, 49}, {85, 56}, {100, 61}, {125, 66}, {150, 71}, {175, 77}, {200, 83},
                    {225, 88}, {250, 94}, {275, 100}, {300, 106}, {350, 119}, {400, 132}, {450, 145}, {500, 158},
                    {550, 172}, {600, 185}, {650, 198}, {700, 211}, {750, 224}, {800, 238}, {850, 251}, {900, 264}
                }
            },
            {"1.25St", new Dictionary<int, int>
                {
                    {50, 37}, {65, 44}, {75, 49}, {85, 56}, {100, 61}, {125, 66}, {150, 71}, {175, 77}, {200, 83},
                    {225, 88}, {250, 94}, {275, 100}, {300, 106}, {350, 119}, {400, 132}, {450, 145}, {500, 158},
                    {550, 172}, {600, 185}, {650, 198}, {700, 211}, {750, 224}, {800, 238}, {850, 251}, {900, 264}
                }
            },
            {"1.25St/B", new Dictionary<int, int>
                {
                    {50, 43}, {65, 52}, {75, 57}, {85, 66}, {100, 71}, {125, 77}, {150, 83}, {175, 90}, {200, 97},
                    {225, 102}, {250, 109}, {275, 116}, {300, 123}, {350, 139}, {400, 154}, {450, 169}, {500, 185},
                    {550, 200}, {600, 216}, {650, 231}, {700, 246}, {750, 262}, {800, 277}, {850, 293}, {900, 308}
                }
            },
            {"1.5St", new Dictionary<int, int>
                {
                    {50, 40}, {65, 48}, {75, 53}, {85, 61}, {100, 66}, {125, 72}, {150, 77}, {175, 83}, {200, 90},
                    {225, 95}, {250, 101}, {275, 108}, {300, 114}, {350, 129}, {400, 143}, {450, 157}, {500, 172},
                    {550, 186}, {600, 200}, {650, 215}, {700, 229}, {750, 243}, {800, 317}, {850, 272}, {900, 286}
                }
            },
            {"1.5St/B", new Dictionary<int, int>
                {
                    {50, 47}, {65, 56}, {75, 62}, {85, 71}, {100, 77}, {125, 83}, {150, 89}, {175, 96}, {200, 104},
                    {225, 110}, {250, 117}, {275, 125}, {300, 132}, {350, 149}, {400, 165}, {450, 182}, {500, 198},
                    {550, 215}, {600, 231}, {650, 248}, {700, 264}, {750, 281}, {800, 297}, {850, 314}, {900, 330}
                }
            },
            {"1.75St", new Dictionary<int, int>
                {
                    {50, 43}, {65, 52}, {75, 57}, {85, 66}, {100, 71}, {125, 77}, {150, 83}, {175, 90}, {200, 97},
                    {225, 102}, {250, 109}, {275, 116}, {300, 123}, {350, 139}, {400, 154}, {450, 169}, {500, 185},
                    {550, 200}, {600, 216}, {650, 231}, {700, 246}, {750, 262}, {800, 277}, {850, 293}, {900, 308}
                }
            },
            {"1.75St/B", new Dictionary<int, int>
                {
                    {50, 50}, {65, 59}, {75, 66}, {85, 75}, {100, 82}, {125, 88}, {150, 94}, {175, 102}, {200, 110},
                    {225, 117}, {250, 125}, {275, 133}, {300, 141}, {350, 158}, {400, 176}, {450, 194}, {500, 211},
                    {550, 229}, {600, 246}, {650, 264}, {700, 282}, {750, 299}, {800, 317}, {850, 334}, {900, 352}
                }
            },
            {"2St", new Dictionary<int, int>
                {
                    {50, 47}, {65, 56}, {75, 62}, {85, 71}, {100, 77}, {125, 83}, {150, 89}, {175, 96}, {200, 104},
                    {225, 110}, {250, 117}, {275, 125}, {300, 132}, {350, 149}, {400, 165}, {450, 182}, {500, 198},
                    {550, 215}, {600, 231}, {650, 248}, {700, 264}, {750, 281}, {800, 297}, {850, 314}, {900, 330}
                }
            },
            {"2St/B", new Dictionary<int, int>
                {
                    {50, 53}, {65, 63}, {75, 70}, {85, 80}, {100, 87}, {125, 94}, {150, 100}, {175, 109}, {200, 117},
                    {225, 124}, {250, 133}, {275, 141}, {300, 150}, {350, 168}, {400, 187}, {450, 206}, {500, 224},
                    {550, 243}, {600, 262}, {650, 281}, {700, 299}, {750, 318}, {800, 337}, {850, 355}, {900, 374}
                }
            }
        };

        private void CalculatePrice(object sender, EventArgs e)
        {
            if (sender is Control control && control.Tag is int rowNumber)
            {
                var typeComboBox = this.Controls.Find($"additiontype{rowNumber}", true).FirstOrDefault() as ComboBox;
                var porchComboBox = this.Controls.Find($"additionsporchtype{rowNumber}", true).FirstOrDefault() as ComboBox;
                var plumbingComboBox = this.Controls.Find($"additionsplumbingtype{rowNumber}", true).FirstOrDefault() as ComboBox;
                var sfTextBox = this.Controls.Find($"additionssf{rowNumber}", true).FirstOrDefault() as TextBox;
                var ssvTextBox = this.Controls.Find($"additionssv{rowNumber}", true).FirstOrDefault() as TextBox;

                int priceForMultiplication = 0;
                int plumbingPrice = 0;

                // Check for Addition Type
                if (!string.IsNullOrEmpty(typeComboBox?.Text) && !string.IsNullOrEmpty(sfTextBox?.Text))
                {
                    string selectedType = typeComboBox.Text;
                    int.TryParse(sfTextBox.Text, out int squareFootage);
                    priceForMultiplication += InterpolatePrice(selectedType, squareFootage);
                }

                // Check for Porch Type
                if (!string.IsNullOrEmpty(porchComboBox?.Text) && !string.IsNullOrEmpty(sfTextBox?.Text))
                {
                    string selectedPorchType = porchComboBox.Text;
                    if (!porchpriceData.ContainsKey(selectedPorchType))
                    {
                        Console.WriteLine($"Porch type data not found for: {selectedPorchType}");  // Logging
                        return;
                    }

                    int porchSquareFootage;
                    if (!int.TryParse(sfTextBox.Text, out porchSquareFootage))
                    {
                        Console.WriteLine($"Failed to parse square footage from sfTextBox: {sfTextBox.Text}");  // Logging
                        return;
                    }

                    priceForMultiplication += InterpolatepPorchPrice(selectedPorchType, porchSquareFootage);
                    Console.WriteLine($"Interpolated price for {selectedPorchType} with {porchSquareFootage} SF: {priceForMultiplication}");  // Logging
                }

                // Check for Plumbing Type without depending on sfTextBox
                if (!string.IsNullOrEmpty(plumbingComboBox?.Text))
                {
                    string selectedType = plumbingComboBox.Text;
                    if (plumbingData.ContainsKey(selectedType))
                        plumbingPrice += plumbingData[selectedType];
                }

                ssvTextBox.Text = (priceForMultiplication * 100 + plumbingPrice).ToString();  // Combine and convert to total price
            }
        }


        private int InterpolatePrice(string type, int sqft)
        {
            if (!priceData.ContainsKey(type))
                return 0;

            var data = priceData[type];
            if (data.ContainsKey(sqft))
                return data[sqft];

            int lowerBound = 0, upperBound = int.MaxValue;
            int lowerBoundValue = 0, upperBoundValue = 0;

            foreach (var entry in data)
            {
                if (entry.Key < sqft && entry.Key > lowerBound)
                {
                    lowerBound = entry.Key;
                    lowerBoundValue = entry.Value;
                }

                if (entry.Key > sqft && entry.Key < upperBound)
                {
                    upperBound = entry.Key;
                    upperBoundValue = entry.Value;
                }
            }

            if (lowerBound == 0)
                return upperBoundValue;  // Return the smallest value if sqft is below the range
            if (upperBound == int.MaxValue)
                return lowerBoundValue;  // Return the largest value if sqft is above the range

            // Linear interpolation
            return lowerBoundValue + (sqft - lowerBound) * (upperBoundValue - lowerBoundValue) / (upperBound - lowerBound);
        }

        private int InterpolatepPorchPrice(string type, int sqft)
        {
            if (!porchpriceData.ContainsKey(type))
                return 0;

            var data = porchpriceData[type];
            if (data.ContainsKey(sqft))
                return data[sqft];

            int lowerBound = 0, upperBound = int.MaxValue;
            int lowerBoundValue = 0, upperBoundValue = 0;

            foreach (var entry in data)
            {
                if (entry.Key < sqft && entry.Key > lowerBound)
                {
                    lowerBound = entry.Key;
                    lowerBoundValue = entry.Value;
                }

                if (entry.Key > sqft && entry.Key < upperBound)
                {
                    upperBound = entry.Key;
                    upperBoundValue = entry.Value;
                }
            }

            if (lowerBound == 0)
                return upperBoundValue;  // Return the smallest value if sqft is below the range
            if (upperBound == int.MaxValue)
                return lowerBoundValue;  // Return the largest value if sqft is above the range

            // Linear interpolation
            return lowerBoundValue + (sqft - lowerBound) * (upperBoundValue - lowerBoundValue) / (upperBound - lowerBound);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Populate ComboBox controls with land types
            #region Land Types

            // Define the list of items
            string[] landtypeitems = {
                "Rural Base",
                "Rural Acreage Pasture",
                "Wooded",
                "Tillable",
                "Well",
                "Septic",
                "Urban Base",
                "Urban Acreage",
                "Softwood TG",
                "Mixed Wood TG",
                "Hardwood TG",
                "Gravel Pits",
                "Roads Class 1",
                "Roads Class 2",
                "Gravel Pit",
                "Wasteland",
                "Commercial Base",
                "Commercial Acreage",
                "Industrial Base",
                "Industrial Acreage"
            };

            // Populate each ComboBox with the items
            landdatatype1.Items.AddRange(landtypeitems);
            landdatatype2.Items.AddRange(landtypeitems);
            landdatatype4.Items.AddRange(landtypeitems);
            landdatatype3.Items.AddRange(landtypeitems);
            landdatatype6.Items.AddRange(landtypeitems);
            landdatatype5.Items.AddRange(landtypeitems);
            landdatatype10.Items.AddRange(landtypeitems);
            landdatatype9.Items.AddRange(landtypeitems);
            landdatatype8.Items.AddRange(landtypeitems);
            landdatatype7.Items.AddRange(landtypeitems);

            #endregion
        }



        // Other methods and event handlers go here

    }
}
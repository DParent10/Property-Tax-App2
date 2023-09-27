using System;
using System.Windows.Forms;

namespace Property_Tax
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

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

        Dictionary<string, double> exemptionValues = new Dictionary<string, double>
            {
                { "Veteran", 6000.0 },
                { "Veteran (Non-Maine Enlisted)", 6000.0 },
                { "Homestead", 25000.0 },
                // ... add other exemption types and values
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

        private void CalculatePrice(object? sender, EventArgs e)
        {
            if (sender is Control control && control.Tag is int rowNumber)
            {
                var typeComboBox = this.Controls.Find($"additiontype{rowNumber}", true).FirstOrDefault() as ComboBox;
                var ssfTextBox = this.Controls.Find($"additionssf{rowNumber}", true).FirstOrDefault() as TextBox;
                var ssvTextBox = this.Controls.Find($"additionssv{rowNumber}", true).FirstOrDefault() as TextBox;

                // If the square footage is empty or the combo box has no selected item, clear the square value.
                if (string.IsNullOrEmpty(ssfTextBox?.Text) || typeComboBox?.SelectedItem == null)
                {
                    ssvTextBox.Text = "";  // Clear the value box.
                    return;
                }

                string selectedType = typeComboBox.SelectedItem.ToString();
                if (!priceData.ContainsKey(selectedType))
                    return;

                int squareFootage;
                if (!int.TryParse(ssfTextBox.Text, out squareFootage))
                    return;

                int price = InterpolatePrice(selectedType, squareFootage);

                ssvTextBox.Text = (price * 100).ToString();  // Convert points to dollars
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

        private void assessmentrecordlv1_TextChanged(object sender, EventArgs e)
        {
            assessmentrecordlv1.TextChanged += UpdateTotal;
        }

        private void additiontype2_SelectedIndexChanged(object sender, EventArgs e)
        {
            additiontype2.SelectedIndexChanged += CalculatePrice;
        }

        private void additionsf2_TextChanged(object sender, EventArgs e)
        {
            additionssf2.TextChanged += CalculatePrice;
        }
        // Other methods and event handlers go here

    }
}

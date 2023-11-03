using System;
using System.Windows.Forms;
using System.Data.SQLite;

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

        private SQLiteConnection GetConnection()
        {
            string connectionString = "Data Source=C:\\Users\\Devin\\source\\repos\\DParent10\\Property-Tax-App2\\Property-Tax-App2\\Property Tax\\database.db;Version=3;";
            return new SQLiteConnection(connectionString);
        }

        private void loadbutton_Click(object sender, EventArgs e)
        {
            using (SQLiteConnection conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT * FROM PropertyInformation WHERE MapNumber = @map OR LotNumber = @lot OR AccountNumber = @account";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@map", maptextbox.Text);
                    cmd.Parameters.AddWithValue("@lot", lottextbox.Text);
                    cmd.Parameters.AddWithValue("@account", accountnumbertext.Text);
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            maptextbox.Text = reader["MapNumber"].ToString();   // Added
                            lottextbox.Text = reader["LotNumber"].ToString();   // Added
                            accountnumbertext.Text = reader["AccountNumber"].ToString();   // Added                                                           // Fill the textboxes with the retrieved data
                            cardtextbox.Text = reader["CardNumber"].ToString();
                            cardstextbox.Text = reader["CardTotal"].ToString();
                            locationnumbertextbox.Text = reader["LocationNumber"].ToString();
                            streetnametextbox.Text = reader["StreetName"].ToString();
                        }
                    }
                }
                // Fetch associated OwnerID from PropertyOwnerJunction using PropertyID
                string getOwnerJunctionQuery = @"SELECT OwnerID FROM PropertyOwnerJunction 
                                 WHERE PropertyID = (SELECT PropertyID FROM PropertyInformation WHERE AccountNumber = @account)";
                using (SQLiteCommand getOwnerJunctionCmd = new SQLiteCommand(getOwnerJunctionQuery, conn))
                {
                    getOwnerJunctionCmd.Parameters.AddWithValue("@account", accountnumbertext.Text);
                    object result = getOwnerJunctionCmd.ExecuteScalar();

                    if (result != null)
                    {
                        int ownerId = Convert.ToInt32(result);

                        // Fetch owner details from OwnerInformation using OwnerID
                        string getOwnerQuery = "SELECT * FROM OwnerInformation WHERE OwnerID = @ownerId";
                        using (SQLiteCommand getOwnerCmd = new SQLiteCommand(getOwnerQuery, conn))
                        {
                            getOwnerCmd.Parameters.AddWithValue("@ownerId", ownerId);
                            using (SQLiteDataReader ownerReader = getOwnerCmd.ExecuteReader())
                            {
                                if (ownerReader.Read())
                                {
                                    // Fill the textboxes with the retrieved data
                                    currentownertextbox.Text = ownerReader["CurrentOwner"].ToString();
                                    secondownertextbox.Text = ownerReader["SecondOwner"].ToString();
                                    ownerstreettextbox.Text = ownerReader["Street1"].ToString();
                                    ownerstreettextbox2.Text = ownerReader["Street2"].ToString();
                                    ownercitytextbox.Text = ownerReader["City"].ToString();
                                    ownerstatetextbox.Text = ownerReader["State"].ToString();
                                    ownerzipcodetextbox.Text = ownerReader["ZipCode"].ToString();
                                }
                            }
                        }
                    }
                }
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            // Clear specified textboxes
            maptextbox.Clear();
            lottextbox.Clear();
            accountnumbertext.Clear();
            cardtextbox.Clear();
            cardstextbox.Clear();
            locationnumbertextbox.Clear();
            streetnametextbox.Clear();
            currentownertextbox.Clear();
            secondownertextbox.Clear();
            ownerstreettextbox.Clear();
            ownerstreettextbox2.Clear();
            ownercitytextbox.Clear();
            ownerstatetextbox.Clear();
            ownerzipcodetextbox.Clear();
        }

        private void deletebutton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to delete this record?", "Confirmation", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                using (SQLiteConnection conn = GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM PropertyInformation WHERE AccountNumber = @account";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@account", accountnumbertext.Text);
                        cmd.ExecuteNonQuery();
                    }
                    // Similar code to delete from OwnerInformation and PropertyOwnerJunction
                }
            }
        }

        private void savebutton_Click(object sender, EventArgs e)
        {
            using (SQLiteConnection conn = GetConnection())
            {
                conn.Open();

                // 1. Check if Record Exists
                string checkQuery = "SELECT COUNT(*) FROM PropertyInformation WHERE AccountNumber = @account";
                using (SQLiteCommand checkCmd = new SQLiteCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@account", accountnumbertext.Text);
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                    // 2. Insert or Update
                    if (count == 0) // Insert new record
                    {
                        string insertQuery = @"
                    INSERT INTO PropertyInformation (MapNumber, LotNumber, AccountNumber, CardNumber, CardTotal, LocationNumber, StreetName)
                    VALUES (@map, @lot, @account, @cardNumber, @cardTotal, @locationNumber, @streetName)";
                        using (SQLiteCommand insertCmd = new SQLiteCommand(insertQuery, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@map", maptextbox.Text);
                            insertCmd.Parameters.AddWithValue("@lot", lottextbox.Text);
                            insertCmd.Parameters.AddWithValue("@account", accountnumbertext.Text);
                            insertCmd.Parameters.AddWithValue("@cardNumber", cardtextbox.Text);
                            insertCmd.Parameters.AddWithValue("@cardTotal", cardstextbox.Text);
                            insertCmd.Parameters.AddWithValue("@locationNumber", locationnumbertextbox.Text);
                            insertCmd.Parameters.AddWithValue("@streetName", streetnametextbox.Text);
                            insertCmd.ExecuteNonQuery();
                        }
                        // Similar code to insert into OwnerInformation and PropertyOwnerJunction
                    }
                    else // Update existing record
                    {
                        string updateQuery = @"
                    UPDATE PropertyInformation 
                    SET MapNumber = @map, LotNumber = @lot, CardNumber = @cardNumber, CardTotal = @cardTotal, LocationNumber = @locationNumber, StreetName = @streetName 
                    WHERE AccountNumber = @account";
                        using (SQLiteCommand updateCmd = new SQLiteCommand(updateQuery, conn))
                        {
                            updateCmd.Parameters.AddWithValue("@map", maptextbox.Text);
                            updateCmd.Parameters.AddWithValue("@lot", lottextbox.Text);
                            updateCmd.Parameters.AddWithValue("@account", accountnumbertext.Text);
                            updateCmd.Parameters.AddWithValue("@cardNumber", cardtextbox.Text);
                            updateCmd.Parameters.AddWithValue("@cardTotal", cardstextbox.Text);
                            updateCmd.Parameters.AddWithValue("@locationNumber", locationnumbertextbox.Text);
                            updateCmd.Parameters.AddWithValue("@streetName", streetnametextbox.Text);
                            updateCmd.ExecuteNonQuery();
                        }
                        // Insert/Update OwnerInformation
                        string checkOwnerQuery = "SELECT OwnerID FROM OwnerInformation WHERE CurrentOwner = @currentOwner AND SecondOwner = @secondOwner";
                        using (SQLiteCommand checkOwnerCmd = new SQLiteCommand(checkOwnerQuery, conn))
                        {
                            checkOwnerCmd.Parameters.AddWithValue("@currentOwner", currentownertextbox.Text);
                            checkOwnerCmd.Parameters.AddWithValue("@secondOwner", secondownertextbox.Text);
                            object result = checkOwnerCmd.ExecuteScalar();

                            int ownerId;
                            if (result == null) // Insert new owner record
                            {
                                string insertOwnerQuery = @"INSERT INTO OwnerInformation (CurrentOwner, SecondOwner, Street1, Street2, City, State, ZipCode)
                                                           VALUES (@currentOwner, @secondOwner, @street1, @street2, @city, @state, @zipCode)";
                                using (SQLiteCommand insertOwnerCmd = new SQLiteCommand(insertOwnerQuery, conn))
                                {
                                    insertOwnerCmd.Parameters.AddWithValue("@currentOwner", currentownertextbox.Text);
                                    insertOwnerCmd.Parameters.AddWithValue("@secondOwner", secondownertextbox.Text);
                                    insertOwnerCmd.Parameters.AddWithValue("@street1", ownerstreettextbox.Text);
                                    insertOwnerCmd.Parameters.AddWithValue("@street2", ownerstreettextbox2.Text);
                                    insertOwnerCmd.Parameters.AddWithValue("@city", ownercitytextbox.Text);
                                    insertOwnerCmd.Parameters.AddWithValue("@state", ownerstatetextbox.Text);
                                    insertOwnerCmd.Parameters.AddWithValue("@zipCode", ownerzipcodetextbox.Text);
                                    insertOwnerCmd.ExecuteNonQuery();

                                    ownerId = (int)conn.LastInsertRowId; // Get the ID of the newly inserted owner
                                }
                            }
                            else // Owner exists, get the ID
                            {
                                ownerId = Convert.ToInt32(result);
                            }

                            // Get PropertyID for the current account number
                            string getPropertyIdQuery = "SELECT PropertyID FROM PropertyInformation WHERE AccountNumber = @account";
                            using (SQLiteCommand getPropertyIdCmd = new SQLiteCommand(getPropertyIdQuery, conn))
                            {
                                getPropertyIdCmd.Parameters.AddWithValue("@account", accountnumbertext.Text);
                                int propertyId = Convert.ToInt32(getPropertyIdCmd.ExecuteScalar());

                                // Insert into PropertyOwnerJunction
                                string insertJunctionQuery = @"INSERT OR IGNORE INTO PropertyOwnerJunction (PropertyID, OwnerID)
                                                             VALUES (@propertyId, @ownerId)";
                                using (SQLiteCommand insertJunctionCmd = new SQLiteCommand(insertJunctionQuery, conn))
                                {
                                    insertJunctionCmd.Parameters.AddWithValue("@propertyId", propertyId);
                                    insertJunctionCmd.Parameters.AddWithValue("@ownerId", ownerId);
                                    insertJunctionCmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
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

        private Dictionary<string, decimal> landTypeToCost = new Dictionary<string, decimal>
        {
            {"", 0m},
            {"Rural Base", 8000.00m},
            {"Rural Acreage", 350.00m},
            {"Urban Base", 10500.00m},
            {"Urban Acreage", 1000.00m},
            {"Wasteland", 75.00m},
            {"Pasture", 350.00m},
            {"Wooded", 350.00m},
            {"Tillable", 450.00m},
            {"Well", 1500.00m},
            {"Septic", 3000.00m},
            {"Softwood TG", 117.00m},
            {"Mixed Wood TG", 172.00m},
            {"Hardwood TG", 188.00m},
            {"Roads Class 1", 1500.00m},
            {"Roads Class 2", 1000.00m},
            {"Gravel Pit", 1000.00m},
            {"Commercial Base", 20000.00m},
            {"Commercial Acreage", 1500.00m},
            {"Industrial Base", 12000.00m},
            {"Industrial Acreage", 1500.00m},
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
                "",
                "Rural Base",
                "Rural Acreage",
                "Pasture",
                "Wooded",
                "Tillable",
                "Well",
                "Septic",
                "Urban Base",
                "Urban Acreage",
                "Softwood TG",
                "Mixed Wood TG",
                "Hardwood TG",
                "Roads Class 1",
                "Roads Class 2",
                "Gravel Pit",
                "Wasteland",
                "Commercial Base",
                "Commercial Acreage",
                "Industrial Base",
                "Industrial Acreage"
            };

            // Populate each ComboBox with the items and assign tags
            ComboBox[] comboBoxes =
            {
                landdatatype1, landdatatype2, landdatatype3, landdatatype4,
                landdatatype5, landdatatype6, landdatatype7, landdatatype8,
                landdatatype9, landdatatype10
            };

            for (int i = 0; i < comboBoxes.Length; i++)
            {
                comboBoxes[i].Items.AddRange(landtypeitems);
                comboBoxes[i].Tag = i + 1;
                comboBoxes[i].SelectedIndexChanged += LandTypeComboBox_SelectedIndexChanged;
            }

            // Attach event handlers for acreage and factor TextBoxes
            TextBox[] acreageTextBoxes =
            {
                landdataacreage1, landdataacreage2, landdataacreage3, landdataacreage4,
                landdataacreage5, landdataacreage6, landdataacreage7, landdataacreage8,
                landdataacreage9, landdataacreage10
            };

            TextBox[] factorTextBoxes =
            {
                landdatafactor1, landdatafactor2, landdatafactor3, landdatafactor4,
                landdatafactor5, landdatafactor6, landdatafactor7, landdatafactor8,
                landdatafactor9, landdatafactor10
            };

            TextBox[] scheduleCostTextBoxes =
            {
                landdataschedulecost1, landdataschedulecost2, landdataschedulecost3, landdataschedulecost4,
                landdataschedulecost5, landdataschedulecost6, landdataschedulecost7, landdataschedulecost8,
                landdataschedulecost9, landdataschedulecost10
            };

            for (int i = 0; i < 10; i++)
            {
                acreageTextBoxes[i].Tag = i + 1;
                factorTextBoxes[i].Tag = i + 1;
                scheduleCostTextBoxes[i].Tag = i + 1;  // Add this line to set the tag
                acreageTextBoxes[i].TextChanged += CalculateAdjustedCost;
                factorTextBoxes[i].TextChanged += CalculateAdjustedCost;
                scheduleCostTextBoxes[i].TextChanged += CalculateAdjustedCost;  // Add this line to attach the handler
            }

            #endregion
        }

        private void LandTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.Tag is int rowNumber)
            {
                string selectedLandType = comboBox.SelectedItem.ToString();

                // Find the cost and set it in the corresponding landdataschedulecost TextBox
                if (landTypeToCost.TryGetValue(selectedLandType, out decimal cost))
                {
                    var costTextBox = this.Controls.Find($"landdataschedulecost{rowNumber}", true).FirstOrDefault() as TextBox;
                    if (costTextBox != null)
                    {
                        costTextBox.Text = cost.ToString("F2");
                    }
                }
                else
                {
                    MessageBox.Show("Error: Unknown land type selected.");
                }
            }
        }

        private static readonly Dictionary<decimal, int> FractionalAcreageToFactor = new Dictionary<decimal, int>
        {
            {1M, 100},
            {0.99M, 100},
            {0.98M, 99},
            {0.97M, 98},
            {0.96M, 97},
            {0.95M, 97},
            {0.94M, 97},
            {0.93M, 96},
            {0.92M, 96},
            {0.91M, 95},
            {0.90M, 95},
            {0.89M, 94},
            {0.88M, 93},
            {0.87M, 93},
            {0.86M, 92},
            {0.85M, 92},
            {0.84M, 92},
            {0.83M, 91},
            {0.82M, 91},
            {0.81M, 90},
            {0.80M, 89},
            {0.79M, 89},
            {0.78M, 88},
            {0.77M, 87},
            {0.76M, 87},
            {0.75M, 87},
            {0.74M, 86},
            {0.73M, 86},
            {0.72M, 85},
            {0.71M, 85},
            {0.70M, 84},
            {0.69M, 83},
            {0.68M, 83},
            {0.67M, 82},
            {0.66M, 81},
            {0.65M, 81},
            {0.64M, 80},
            {0.63M, 79},
            {0.62M, 79},
            {0.61M, 78},
            {0.60M, 77},
            {0.59M, 76},
            {0.58M, 76},
            {0.57M, 76},
            {0.56M, 75},
            {0.55M, 74},
            {0.54M, 73},
            {0.53M, 72},
            {0.52M, 72},
            {0.51M, 71},
            {0.50M, 71},
            {0.49M, 70},
            {0.48M, 68},
            {0.47M, 68},
            {0.46M, 67},
            {0.45M, 67},
            {0.44M, 66},
            {0.43M, 66},
            {0.42M, 65},
            {0.41M, 64},
            {0.40M, 63},
            {0.39M, 62},
            {0.38M, 62},
            {0.37M, 61},
            {0.36M, 60},
            {0.35M, 59},
            {0.34M, 58},
            {0.33M, 57},
            {0.32M, 56},
            {0.31M, 55},
            {0.30M, 54},
            {0.29M, 53},
            {0.28M, 52},
            {0.27M, 51},
            {0.26M, 50},
            {0.25M, 50},
            {0.24M, 49},
            {0.23M, 48},
            {0.22M, 47},
            {0.21M, 46},
            {0.20M, 45},
            {0.19M, 44},
            {0.18M, 43},
            {0.17M, 42},
            {0.16M, 40},
            {0.15M, 39},
            {0.14M, 38},
            {0.13M, 37},
            {0.12M, 35},
            {0.11M, 33},
            {0.10M, 32},
            {0.09M, 30},
            {0.08M, 27},
            {0.07M, 23},
            {0.06M, 20},
            {0.05M, 18},
            {0.04M, 16},
            {0.03M, 14},
            {0.02M, 12},
            {0.01M, 10}
        };

        private void CalculateAdjustedCost(object sender, EventArgs e)
        {
            if (sender is TextBox inputBox && inputBox.Tag is int rowNumber)
            {
                // Get the ComboBox for this row
                var landTypeComboBox = this.Controls.Find($"landdatatype{rowNumber}", true).FirstOrDefault() as ComboBox;
                if (landTypeComboBox == null) return;

                // Get values from TextBoxes
                var acreageTextBox = this.Controls.Find($"landdataacreage{rowNumber}", true).FirstOrDefault() as TextBox;
                var scheduleCostTextBox = this.Controls.Find($"landdataschedulecost{rowNumber}", true).FirstOrDefault() as TextBox;
                var factorTextBox = this.Controls.Find($"landdatafactor{rowNumber}", true).FirstOrDefault() as TextBox;
                var adjustedCostTextBox = this.Controls.Find($"landdataadjustedcost{rowNumber}", true).FirstOrDefault() as TextBox;
                var factorReasonTextBox = this.Controls.Find($"landdatafactorreason{rowNumber}", true).FirstOrDefault() as TextBox;

                if (scheduleCostTextBox == null || factorTextBox == null || adjustedCostTextBox == null || factorReasonTextBox == null) return;

                decimal acreage = 0, scheduleCost = 0, factor = 0, adjustedCost = 0;

                string selectedLandType = landTypeComboBox.SelectedItem.ToString();

                if (selectedLandType == "Well" || selectedLandType == "Septic")
                {
                    decimal.TryParse(scheduleCostTextBox.Text, out scheduleCost);
                    decimal.TryParse(factorTextBox.Text, out factor);
                    adjustedCost = scheduleCost * factor / 100;
                }
                else
                {
                    // Parse necessary variables
                    decimal.TryParse(acreageTextBox.Text, out acreage);
                    decimal.TryParse(scheduleCostTextBox.Text, out scheduleCost);
                    decimal.TryParse(factorTextBox.Text, out factor);

                    // Check for fractional acreage logic
                    if (new[] { "Rural Base", "Urban Base", "Commercial Base", "Industrial Base" }.Contains(selectedLandType) && acreage <= 1)
                    {
                        // Get the factor based on fractional acreage chart
                        if (new[] { "Rural Base", "Urban Base", "Commercial Base", "Industrial Base" }.Contains(selectedLandType))
                        {
                            if (acreage == 1)
                            {
                                factorTextBox.Text = "100";
                                factorReasonTextBox.Text = "";  // Clear the factor reason
                                factor = 100;  // Update the factor variable for subsequent calculations
                            }
                            else if (acreage < 1)
                            {
                                // Get the factor based on fractional acreage chart
                                if (FractionalAcreageToFactor.TryGetValue(Math.Round(acreage, 2), out int fractionalFactor))
                                {
                                    factorTextBox.Text = fractionalFactor.ToString();
                                    factorReasonTextBox.Text = "Fractional Acreage";  // Update factor reason
                                    factor = fractionalFactor; // update the factor variable for subsequent calculations
                                }
                            }
                        }
                    }

                    if (new[] { "Rural Base", "Urban Base", "Commercial Base", "Industrial Base" }.Contains(selectedLandType))
                    {
                        adjustedCost = scheduleCost * factor / 100;
                    }
                    else
                    {
                        adjustedCost = acreage * scheduleCost * factor / 100;
                    }
                }

                // Round to the nearest hundred
                adjustedCost = Math.Round(adjustedCost / 100) * 100;

                adjustedCostTextBox.Text = adjustedCost.ToString("F2");
            }

            UpdateTotalAcreage();
        }

        private void UpdateTotalAcreage()
        {
            decimal totalAcreage = 0;
            for (int i = 1; i <= 10; i++)
            {
                var acreageTextBox = this.Controls.Find($"landdataacreage{i}", true).FirstOrDefault() as TextBox;
                if (acreageTextBox != null && decimal.TryParse(acreageTextBox.Text, out decimal acreage))
                {
                    totalAcreage += acreage;
                }
            }
            landdataacreagetotal1.Text = totalAcreage.ToString("F2");
        }

    }
}
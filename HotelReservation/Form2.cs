using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace HotelReservation
{
    public partial class Form2 : Form
    {
        private Label lblDashboard;

        // Filter Controls
        private ComboBox cmbRoomCategory;
        private ComboBox cmbBedCount;
        private Button btnFilterRooms;
        private Button btnResetFilter;
        private Label lblMatchNotice;

        private class RoomCard
        {
            public string Category { get; set; }  // "Standard", "Deluxe", "Suite"
            public int Beds { get; set; }         // 1, 2, 3
            public decimal Price { get; set; }
            public string Title { get; set; }
            public Control ImageBox { get; set; }
            public Control PricePanel { get; set; }
            public Control InfoBox { get; set; }
            public Control AccentLine { get; set; }
            public Button BookButton { get; set; }

            public void SetVisible(bool visible)
            {
                if (ImageBox != null) ImageBox.Visible = visible;
                if (PricePanel != null) PricePanel.Visible = visible;
                if (InfoBox != null) InfoBox.Visible = visible;
                if (AccentLine != null) AccentLine.Visible = visible;
                if (BookButton != null) BookButton.Visible = visible;
            }

            public void SetPosition(int x, int y)
            {
                int cardWidth = 234;
                int imgHeight = 175;
                int barHeight = 34;
                int btnWidth = 75;
                int badgeWidth = cardWidth - btnWidth; // 159px

                // 1. Bed Image Box
                if (ImageBox != null)
                {
                    ImageBox.Size = new Size(cardWidth, imgHeight);
                    ImageBox.Location = new Point(x, y);
                    ImageBox.BringToFront();
                }

                // 2. Action Bar (Book Button + Price Banner)
                int barY = y + imgHeight;
                if (BookButton != null)
                {
                    BookButton.Size = new Size(btnWidth, barHeight);
                    BookButton.Location = new Point(x, barY);
                    BookButton.BringToFront();
                }

                if (PricePanel != null)
                {
                    PricePanel.Size = new Size(badgeWidth, barHeight);
                    PricePanel.Location = new Point(x + btnWidth, barY);
                    PricePanel.BringToFront();
                }

                // 3. Info Box (Beds & Area)
                int infoY = barY + barHeight;
                if (InfoBox != null)
                {
                    InfoBox.Size = new Size(cardWidth, 58);
                    InfoBox.Location = new Point(x, infoY);
                    InfoBox.BringToFront();
                }

                // 4. Gold Accent Line
                if (AccentLine != null)
                {
                    AccentLine.Size = new Size(cardWidth, 4);
                    AccentLine.Location = new Point(x, infoY + 58);
                    AccentLine.BringToFront();
                }
            }
        }

        private List<RoomCard> _cards = new List<RoomCard>();

        public Form2()
        {
            InitializeComponent();
            this.AutoSize = false;
            this.WindowState = FormWindowState.Maximized;

            SetupDashboardHeaderLink();
            BuildFull20RoomCatalog();
            SetupCustomFilterPanel();
            ApplyCustomCurvature();
            WireUpNavigation();

            this.Load += (s, e) => ArrangeFilteredCards();
            this.Resize += (s, e) => ArrangeFilteredCards();
        }

        private void SetupDashboardHeaderLink()
        {
            lblDashboard = new Label
            {
                Text = "Dashboard",
                Font = new Font("Georgia", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(61, 42, 48),
                AutoSize = true,
                Cursor = Cursors.Hand
            };

            lblDashboard.Click += (s, e) =>
            {
                AdminLoginForm.OpenDashboard(this);
            };

            groupBox2.Controls.Add(lblDashboard);
        }

        private void BuildFull20RoomCatalog()
        {
            // Maps all 20 rooms present in form2designer.cs
            _cards = new List<RoomCard>()
            {
                // Row 1
                new RoomCard { Category = "Standard", Beds = 1, Price = 3200m, Title = "Standard Room (1 Bed)", ImageBox = groupBox4, PricePanel = panel2, InfoBox = groupBox10, AccentLine = panel8, BookButton = button1 },
                new RoomCard { Category = "Deluxe", Beds = 1, Price = 4200m, Title = "Deluxe Room (1 Bed)", ImageBox = groupBox6, PricePanel = panel9, InfoBox = groupBox5, AccentLine = panel3, BookButton = button2 },
                new RoomCard { Category = "Suite", Beds = 3, Price = 7200m, Title = "Family Suite (3 Bed)", ImageBox = groupBox12, PricePanel = panel10, InfoBox = groupBox11, AccentLine = panel4, BookButton = button4 },
                new RoomCard { Category = "Suite", Beds = 3, Price = 7200m, Title = "Presidential Suite (3 Bed)", ImageBox = groupBox26, PricePanel = panel23, InfoBox = groupBox25, AccentLine = panel22, BookButton = button13 },

                // Row 2
                new RoomCard { Category = "Standard", Beds = 1, Price = 3200m, Title = "Standard Room (1 Bed)", ImageBox = groupBox13, PricePanel = panel11, InfoBox = groupBox7, AccentLine = panel5, BookButton = button5 },
                new RoomCard { Category = "Deluxe", Beds = 1, Price = 4200m, Title = "Deluxe Room (1 Bed)", ImageBox = groupBox14, PricePanel = panel12, InfoBox = groupBox8, AccentLine = panel6, BookButton = button6 },
                new RoomCard { Category = "Suite", Beds = 3, Price = 7200m, Title = "Executive Suite (3 Bed)", ImageBox = groupBox15, PricePanel = panel13, InfoBox = groupBox9, AccentLine = panel7, BookButton = button7 },
                new RoomCard { Category = "Standard", Beds = 1, Price = 3200m, Title = "Standard Room (1 Bed)", ImageBox = groupBox18, PricePanel = panel15, InfoBox = groupBox17, AccentLine = panel14, BookButton = button10 },

                // Row 3
                new RoomCard { Category = "Standard", Beds = 1, Price = 3200m, Title = "Standard Room (1 Bed)", ImageBox = groupBox20, PricePanel = panel17, InfoBox = groupBox19, AccentLine = panel16, BookButton = button11 },
                new RoomCard { Category = "Deluxe", Beds = 1, Price = 4200m, Title = "Deluxe Room (1 Bed)", ImageBox = groupBox24, PricePanel = panel21, InfoBox = groupBox23, AccentLine = panel20, BookButton = button9 },
                new RoomCard { Category = "Suite", Beds = 3, Price = 7200m, Title = "Royal Suite (3 Bed)", ImageBox = groupBox28, PricePanel = panel25, InfoBox = groupBox27, AccentLine = panel24, BookButton = button14 },
                new RoomCard { Category = "Suite", Beds = 3, Price = 7200m, Title = "Grand Suite (3 Bed)", ImageBox = groupBox30, PricePanel = panel27, InfoBox = groupBox29, AccentLine = panel26, BookButton = button15 },

                // Row 4
                new RoomCard { Category = "Standard", Beds = 1, Price = 3200m, Title = "Standard Room (1 Bed)", ImageBox = groupBox22, PricePanel = panel19, InfoBox = groupBox21, AccentLine = panel18, BookButton = button12 },
                new RoomCard { Category = "Suite", Beds = 3, Price = 7200m, Title = "Master Suite (3 Bed)", ImageBox = groupBox32, PricePanel = panel29, InfoBox = groupBox31, AccentLine = panel28, BookButton = button16 },
                new RoomCard { Category = "Suite", Beds = 3, Price = 7200m, Title = "Diplomatic Suite (3 Bed)", ImageBox = groupBox34, PricePanel = panel31, InfoBox = groupBox33, AccentLine = panel30, BookButton = button17 },
                new RoomCard { Category = "Suite", Beds = 3, Price = 7200m, Title = "Imperial Suite (3 Bed)", ImageBox = groupBox36, PricePanel = panel33, InfoBox = groupBox35, AccentLine = panel32, BookButton = button18 },

                // Row 5
                new RoomCard { Category = "Suite", Beds = 3, Price = 7200m, Title = "Penthouse Suite (3 Bed)", ImageBox = groupBox38, PricePanel = panel35, InfoBox = groupBox37, AccentLine = panel34, BookButton = button19 },
                new RoomCard { Category = "Suite", Beds = 3, Price = 7200m, Title = "Crown Suite (3 Bed)", ImageBox = groupBox40, PricePanel = panel37, InfoBox = groupBox39, AccentLine = panel36, BookButton = button20 },
                new RoomCard { Category = "Suite", Beds = 3, Price = 7200m, Title = "Ambassador Suite (3 Bed)", ImageBox = groupBox42, PricePanel = panel39, InfoBox = groupBox41, AccentLine = panel38, BookButton = button21 },
                new RoomCard { Category = "Suite", Beds = 3, Price = 7200m, Title = "Presidential Villa (3 Bed)", ImageBox = groupBox44, PricePanel = panel41, InfoBox = groupBox43, AccentLine = panel40, BookButton = button22 }
            };
        }

        private void SetupCustomFilterPanel()
        {
            panel1.Controls.Clear();

            Label lblCat = new Label
            {
                Text = "Room Category",
                Font = new Font("Georgia", 10.5f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Location = new Point(40, 32),
                AutoSize = true
            };
            panel1.Controls.Add(lblCat);

            cmbRoomCategory = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Georgia", 10.5f),
                Location = new Point(40, 68),
                Size = new Size(200, 32)
            };
            cmbRoomCategory.Items.AddRange(new object[] { "All Categories", "Standard", "Deluxe", "Suite" });
            cmbRoomCategory.SelectedIndex = 0;
            panel1.Controls.Add(cmbRoomCategory);

            Label lblBeds = new Label
            {
                Text = "Number of Beds",
                Font = new Font("Georgia", 10.5f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Location = new Point(280, 32),
                AutoSize = true
            };
            panel1.Controls.Add(lblBeds);

            cmbBedCount = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Georgia", 10.5f),
                Location = new Point(280, 68),
                Size = new Size(180, 32)
            };
            cmbBedCount.Items.AddRange(new object[] { "Any Bed Count", "1 Bed", "2 Beds", "3 Beds" });
            cmbBedCount.SelectedIndex = 0;
            panel1.Controls.Add(cmbBedCount);

            btnFilterRooms = new Button
            {
                Text = "Find Best Option",
                Font = new Font("Georgia", 11f, FontStyle.Bold),
                BackColor = Color.FromArgb(247, 227, 200),
                ForeColor = Color.FromArgb(61, 42, 48),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(500, 48),
                Size = new Size(210, 56),
                Cursor = Cursors.Hand
            };
            btnFilterRooms.FlatAppearance.BorderSize = 0;
            btnFilterRooms.Click += (s, e) => ExecuteFilter();
            panel1.Controls.Add(btnFilterRooms);
            ApplyRoundedRegion(btnFilterRooms, 14);

            btnResetFilter = new Button
            {
                Text = "Show All",
                Font = new Font("Georgia", 10f, FontStyle.Bold),
                BackColor = Color.FromArgb(122, 31, 62),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(730, 48),
                Size = new Size(130, 56),
                Cursor = Cursors.Hand
            };
            btnResetFilter.FlatAppearance.BorderSize = 0;
            btnResetFilter.Click += (s, e) =>
            {
                cmbRoomCategory.SelectedIndex = 0;
                cmbBedCount.SelectedIndex = 0;
                ExecuteFilter();
            };
            panel1.Controls.Add(btnResetFilter);
            ApplyRoundedRegion(btnResetFilter, 14);

            lblMatchNotice = new Label
            {
                Text = "Showing all 20 luxury accommodations.",
                Font = new Font("Georgia", 9.5f, FontStyle.Italic),
                ForeColor = Color.FromArgb(247, 227, 200),
                BackColor = Color.Transparent,
                Location = new Point(42, 114),
                AutoSize = true
            };
            panel1.Controls.Add(lblMatchNotice);
        }

        private void ExecuteFilter()
        {
            string chosenCategory = cmbRoomCategory.SelectedItem?.ToString() ?? "All Categories";
            string chosenBeds = cmbBedCount.SelectedItem?.ToString() ?? "Any Bed Count";

            int targetBeds = 0;
            if (chosenBeds == "1 Bed") targetBeds = 1;
            else if (chosenBeds == "2 Beds") targetBeds = 2;
            else if (chosenBeds == "3 Beds") targetBeds = 3;

            var exactMatches = _cards.Where(c =>
                (chosenCategory == "All Categories" || c.Category.Equals(chosenCategory, StringComparison.OrdinalIgnoreCase)) &&
                (targetBeds == 0 || c.Beds == targetBeds)
            ).ToList();

            if (exactMatches.Count > 0)
            {
                foreach (var c in _cards)
                {
                    c.SetVisible(exactMatches.Contains(c));
                }
                lblMatchNotice.Text = $"Found {exactMatches.Count} matching room(s).";
            }
            else
            {
                var fallbackMatches = _cards.Where(c =>
                    (targetBeds != 0 && c.Beds >= targetBeds) ||
                    (chosenCategory != "All Categories" && c.Category.Equals(chosenCategory, StringComparison.OrdinalIgnoreCase))
                ).ToList();

                if (fallbackMatches.Count == 0) fallbackMatches = _cards;

                foreach (var c in _cards)
                {
                    c.SetVisible(fallbackMatches.Contains(c));
                }
                lblMatchNotice.Text = $"No exact match for {chosenCategory} ({chosenBeds}). Showing best alternatives.";
            }

            ArrangeFilteredCards();
        }

        private void ArrangeFilteredCards()
        {
            this.SuspendLayout();

            int scrollX = this.AutoScrollPosition.X;
            int scrollY = this.AutoScrollPosition.Y;
            int clientWidth = this.ClientSize.Width;

            // 1. Top Contact Bar
            if (groupBox1 != null)
            {
                Control lbl8 = groupBox1.Controls["label8"];
                Control lbl9 = groupBox1.Controls["label9"];
                Control lbl10 = groupBox1.Controls["label10"];

                if (lbl8 != null && lbl9 != null && lbl10 != null)
                {
                    int itemSpacing = 35;
                    lbl10.Left = groupBox1.Width - lbl10.Width - 40;
                    lbl9.Left = lbl10.Left - lbl9.Width - itemSpacing;
                    lbl8.Left = lbl9.Left - lbl8.Width - itemSpacing;

                    int barCenterY = (groupBox1.Height - lbl8.Height) / 2;
                    lbl8.Top = barCenterY;
                    lbl9.Top = barCenterY;
                    lbl10.Top = barCenterY;
                }
            }

            // 2. Navigation Header
            button3.Left = groupBox2.Width - button3.Width - 40;
            if (lblDashboard != null)
            {
                lblDashboard.Left = button3.Left - 105;
                lblDashboard.Top = label6.Top;
                label6.Left = lblDashboard.Left - 85;
            }
            else
            {
                label6.Left = button3.Left - 90;
            }
            label7.Left = label6.Left - 140;
            label5.Left = label7.Left - 75;

            // 3. Center titles and availability panel
            label2.Left = (clientWidth - label2.Width) / 2 + scrollX;
            label3.Left = (clientWidth - label3.Width) / 2 + scrollX;
            panel1.Left = (clientWidth - panel1.Width) / 2 + scrollX;

            // 4. Adaptive Grid Calculation
            var visibleCards = _cards.Where(c => c.ImageBox != null && c.ImageBox.Visible).ToList();
            if (visibleCards.Count == 0)
            {
                this.ResumeLayout(true);
                return;
            }

            int cardWidth = 234;
            int cardTotalHeight = 275 + 28;
            int minGap = 24;

            int maxCols = 4;
            if (clientWidth < 1180) maxCols = 3;
            if (clientWidth < 890) maxCols = 2;
            if (clientWidth < 580) maxCols = 1;

            int numCols = Math.Min(maxCols, visibleCards.Count);
            if (numCols <= 0) numCols = 1;

            int availableWidth = clientWidth - 80;
            int totalCardsWidth = numCols * cardWidth;
            int actualGap = minGap;

            if (numCols > 1)
            {
                actualGap = Math.Max(minGap, (availableWidth - totalCardsWidth) / (numCols - 1));
                if (actualGap > 50) actualGap = 50;
            }

            int totalGridWidth = (numCols * cardWidth) + ((numCols - 1) * actualGap);
            int startX = Math.Max(40, (clientWidth - totalGridWidth) / 2) + scrollX;
            int startY = 700 + scrollY;

            for (int i = 0; i < visibleCards.Count; i++)
            {
                int col = i % numCols;
                int row = i / numCols;

                int posX = startX + col * (cardWidth + actualGap);
                int posY = startY + row * cardTotalHeight;

                visibleCards[i].SetPosition(posX, posY);
            }

            // 5. Position Footer underneath the last row
            int totalRows = (int)Math.Ceiling((double)visibleCards.Count / numCols);
            if (groupBox16 != null)
            {
                groupBox16.Top = startY - scrollY + (totalRows * cardTotalHeight) + 40;
                groupBox16.Width = clientWidth;
            }

            this.ResumeLayout(true);
        }

        private void WireUpNavigation()
        {
            label5.Cursor = Cursors.Hand;
            label6.Cursor = Cursors.Hand;
            label7.Cursor = Cursors.Hand;

            label5.Click += Home_Click;
            button3.Click += (s, e) => OpenReservation("Deluxe Room", 4200m);
            label6.Click += (s, e) => OpenReservation("Standard Room", 3200m);

            foreach (var card in _cards)
            {
                if (card.BookButton != null)
                {
                    card.BookButton.Click += (s, e) => OpenReservation(card.Title, card.Price);
                }
            }
        }

        private void OpenReservation(string roomType, decimal ratePerNight)
        {
            Form3 reservationForm = new Form3(roomType, ratePerNight);
            this.Hide();
            reservationForm.Show();
            reservationForm.FormClosed += (s, args) => this.Show();
        }

        private void Home_Click(object sender, EventArgs e)
        {
            Home homeForm = null;
            foreach (Form openForm in Application.OpenForms)
            {
                if (openForm is Home)
                {
                    homeForm = (Home)openForm;
                    break;
                }
            }

            if (homeForm != null)
            {
                homeForm.Show();
                homeForm.BringToFront();
            }
            else
            {
                new Home().Show();
            }

            this.Close();
        }

        private void ApplyCustomCurvature()
        {
            int buttonRadius = 6;
            int cardRadius = 12;

            foreach (var card in _cards)
            {
                if (card.BookButton != null) ApplyRoundedRegion(card.BookButton, buttonRadius);
                if (card.ImageBox != null) ApplyRoundedRegion(card.ImageBox, cardRadius);
                if (card.InfoBox != null) ApplyRoundedRegion(card.InfoBox, cardRadius);
                if (card.PricePanel != null) ApplyRoundedRegion(card.PricePanel, buttonRadius);
            }

            ApplyRoundedRegion(panel1, 14);
            ApplyRoundedRegion(pictureBox1, 14);
        }

        private void ApplyRoundedRegion(Control control, int radius)
        {
            if (control == null) return;
            control.Resize += (s, e) => UpdateRegion(control, radius);
            UpdateRegion(control, radius);
        }

        private void UpdateRegion(Control control, int radius)
        {
            if (control.Width <= 0 || control.Height <= 0) return;

            using (GraphicsPath path = GetRoundedRectanglePath(control.ClientRectangle, radius))
            {
                control.Region = new Region(path);
            }
        }

        private GraphicsPath GetRoundedRectanglePath(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            GraphicsPath path = new GraphicsPath();

            if (radius <= 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            Rectangle arc = new Rectangle(bounds.Location, new Size(diameter, diameter));
            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void groupBox2_Enter(object sender, EventArgs e) { }
        private void Form2_Load(object sender, EventArgs e) { }
    }
}

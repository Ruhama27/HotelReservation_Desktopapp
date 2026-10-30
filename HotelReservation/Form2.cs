using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HotelReservation
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            this.AutoSize = false;
            this.WindowState = FormWindowState.Maximized;

            ApplyCustomCurvature();
            WireUpNavigation();

            this.Resize += (s, e) => CenterContent();
            CenterContent();
        }

        private void WireUpNavigation()
        {
            label5.Cursor = Cursors.Hand;
            label6.Cursor = Cursors.Hand;
            label7.Cursor = Cursors.Hand;

            label5.Click += Home_Click;
            button3.Click += BookNow_Click;
            label6.Click += BookNow_Click;
            button8.Click += CheckAvailability_Click;

            Control[] roomBookButtons = new Control[]
            {
                button1, button2, button4, button5, button6, button7
            };

            foreach (var btn in roomBookButtons)
            {
                if (btn != null)
                {
                    btn.Click += BookNow_Click;
                }
            }
        }

        private void CenterContent()
        {
            int rightMargin = this.ClientSize.Width - 50;

            // --- Safe Top Contact Bar (groupBox1) ---
            if (groupBox1 != null)
            {
                Control lbl8 = groupBox1.Controls["label8"];
                Control lbl9 = groupBox1.Controls["label9"];
                Control lbl10 = groupBox1.Controls["label10"];

                if (lbl8 != null && lbl9 != null && lbl10 != null)
                {
                    int itemSpacing = 35;
                    lbl10.Left = rightMargin - lbl10.Width;
                    lbl9.Left = lbl10.Left - lbl9.Width - itemSpacing;
                    lbl8.Left = lbl9.Left - lbl8.Width - itemSpacing;

                    int barCenterY = (groupBox1.Height - lbl8.Height) / 2;
                    lbl8.Top = barCenterY;
                    lbl9.Top = barCenterY;
                    lbl10.Top = barCenterY;
                }
            }

            // --- Header Nav Controls (groupBox2) ---
            button3.Left = rightMargin - button3.Width;
            label6.Left = button3.Left - 90;
            label7.Left = label6.Left - 130;
            label5.Left = label7.Left - 80;

            // Center titles and availability panel
            label2.Left = (this.ClientSize.Width - label2.Width) / 2;
            label3.Left = (this.ClientSize.Width - label3.Width) / 2;
            panel1.Left = (this.ClientSize.Width - panel1.Width) / 2;

            // Center room card columns evenly
            int cardWidth = 234;
            int totalBlockWidth = 3 * cardWidth + 2 * 120;
            int startX = Math.Max(40, (this.ClientSize.Width - totalBlockWidth) / 2);
            int colSpacing = (this.ClientSize.Width - (startX * 2) - (3 * cardWidth)) / 2;
            if (colSpacing < 40) colSpacing = 40;

            int col1 = startX;
            int col2 = startX + cardWidth + colSpacing;
            int col3 = startX + (cardWidth + colSpacing) * 2;

            // Column 1
            groupBox4.Left = col1;
            panel2.Left = col1 + 34;
            groupBox10.Left = col1;
            panel8.Left = col1;
            button1.Left = col1 - 62;

            groupBox13.Left = col1;
            panel11.Left = col1 + 34;
            groupBox7.Left = col1;
            panel5.Left = col1;
            button5.Left = col1 - 62;

            // Column 2
            groupBox6.Left = col2;
            panel9.Left = col2 + 34;
            groupBox5.Left = col2;
            panel3.Left = col2;
            button2.Left = col2 - 62;

            groupBox14.Left = col2;
            panel12.Left = col2 + 34;
            groupBox8.Left = col2;
            panel6.Left = col2;
            button6.Left = col2 - 62;

            // Column 3
            groupBox12.Left = col3;
            panel10.Left = col3 + 34;
            groupBox11.Left = col3;
            panel4.Left = col3;
            button4.Left = col3 - 62;

            groupBox15.Left = col3;
            panel13.Left = col3 + 34;
            groupBox9.Left = col3;
            panel7.Left = col3;
            button7.Left = col3 - 62;
        }

        private void CheckAvailability_Click(object sender, EventArgs e)
        {
            Form4 dashboardForm = new Form4();
            this.Hide();
            dashboardForm.Show();
            dashboardForm.FormClosed += (s, args) => this.Show();
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

        private void BookNow_Click(object sender, EventArgs e)
        {
            Form3 reservationForm = new Form3();
            this.Hide();
            reservationForm.Show();
            reservationForm.FormClosed += (s, args) => this.Show();
        }

        private void ApplyCustomCurvature()
        {
            int buttonRadius = 14;
            int cardRadius = 18;
            int smallBoxRadius = 10;

            Control[] buttons = new Control[]
            {
                button1, button2, button3, button4, button5,
                button6, button7, button8, button9
            };
            foreach (var btn in buttons)
            {
                ApplyRoundedRegion(btn, buttonRadius);
            }

            ApplyRoundedRegion(panel1, cardRadius);
            ApplyRoundedRegion(pictureBox1, 16);

            Control[] imageBoxes = new Control[]
            {
                groupBox4, groupBox6, groupBox12,
                groupBox13, groupBox14, groupBox15
            };
            foreach (var imgBox in imageBoxes)
            {
                ApplyRoundedRegion(imgBox, cardRadius);
            }

            Control[] infoBoxes = new Control[]
            {
                groupBox10, groupBox5, groupBox11,
                groupBox7, groupBox8, groupBox9
            };
            foreach (var box in infoBoxes)
            {
                ApplyRoundedRegion(box, cardRadius);
            }

            Control[] priceBadges = new Control[]
            {
                panel2, panel9, panel10,
                panel11, panel12, panel13
            };
            foreach (var badge in priceBadges)
            {
                ApplyRoundedRegion(badge, smallBoxRadius);
            }
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
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HotelReservation
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
            this.AutoSize = false;
            this.WindowState = FormWindowState.Maximized;

            WireUpSidebarNavigation();
            SetupDashboardResponsiveness();

            this.Resize += (s, e) => AdjustDashboard();
        }

        private void WireUpSidebarNavigation()
        {
            label21.Cursor = Cursors.Hand; // Home
            label28.Cursor = Cursors.Hand; // Rooms and Suites
            label22.Cursor = Cursors.Hand; // Reservation
            label20.Cursor = Cursors.Hand; // Dashboard

            label21.Click += (s, e) => NavigateToHome();
            label28.Click += (s, e) => NavigateToRooms();
            label22.Click += (s, e) => NavigateToReservation();
            button3.Click += (s, e) => NavigateToReservation(); // "New reservation" button
        }

        private void SetupDashboardResponsiveness()
        {
            groupBox1.Dock = DockStyle.Left; // Full height pinned sidebar
            button3.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            AdjustDashboard();
        }

        private void AdjustDashboard()
        {
            int sidebarWidth = groupBox1.Width;
            int contentStartX = sidebarWidth + 40;
            int availableWidth = this.ClientSize.Width - contentStartX - 40;

            if (availableWidth < 600) availableWidth = 600;

            // Distribute the 4 statistic metric cards across the top
            int spacing = 20;
            int cardWidth = (availableWidth - (3 * spacing)) / 4;

            groupBox2.Width = cardWidth;
            groupBox3.Width = cardWidth;
            groupBox4.Width = cardWidth;
            groupBox6.Width = cardWidth;

            groupBox2.Left = contentStartX;
            groupBox3.Left = contentStartX + cardWidth + spacing;
            groupBox4.Left = contentStartX + (cardWidth + spacing) * 2;
            groupBox6.Left = contentStartX + (cardWidth + spacing) * 3;

            // Two main lower tables (Arrivals & Rooms free)
            panel2.Left = contentStartX;
            panel2.Width = (int)(availableWidth * 0.60);

            groupBox5.Left = panel2.Right + 30;
            groupBox5.Width = availableWidth - panel2.Width - 30;
        }

        private void NavigateToHome()
        {
            Home home = null;
            foreach (Form openForm in Application.OpenForms)
            {
                if (openForm is Home)
                {
                    home = (Home)openForm;
                    break;
                }
            }

            if (home != null)
            {
                home.Show();
                home.BringToFront();
            }
            else
            {
                new Home().Show();
            }

            this.Close();
        }

        private void NavigateToRooms()
        {
            Form2 rooms = new Form2();
            this.Hide();
            rooms.Show();
            rooms.FormClosed += (s, args) => this.Show();
        }

        private void NavigateToReservation()
        {
            Form3 reservation = new Form3();
            this.Hide();
            reservation.Show();
            reservation.FormClosed += (s, args) => this.Show();
        }

        private void label19_Click(object sender, EventArgs e) { }
        private void label40_Click(object sender, EventArgs e) { }
        private void Form4_Load(object sender, EventArgs e) { }
    }
}
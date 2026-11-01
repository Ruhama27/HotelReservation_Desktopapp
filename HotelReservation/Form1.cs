using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HotelReservation
{
    public partial class Home : Form
    {
        private Label lblDashboard;

        public Home()
        {
            InitializeComponent();
            this.AutoSize = false;
            this.WindowState = FormWindowState.Maximized;

            SetupDashboardHeaderLink();
            ApplyRoundedCorners();
            WireUpNavigation();
            SetupResponsiveness();

            this.Resize += (s, e) => AdjustLayout();
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

        private void SetupResponsiveness()
        {
            groupBox3.Dock = DockStyle.Fill;

            button3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label5.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label6.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label7.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            label3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            AdjustLayout();
        }

        private void AdjustLayout()
        {
            int rightMargin = this.ClientSize.Width - 40;

            // Header navigation: Home -> Rooms and Suites -> Services -> Dashboard -> Book Now
            button3.Left = rightMargin - button3.Width;

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

            // Safe lookup for footer contacts
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

                    int centerY = (groupBox1.Height - lbl8.Height) / 2;
                    lbl8.Top = centerY;
                    lbl9.Top = centerY;
                    lbl10.Top = centerY;
                }
            }
        }

        private void WireUpNavigation()
        {
            label5.Cursor = Cursors.Hand;
            label6.Cursor = Cursors.Hand;
            label7.Cursor = Cursors.Hand;

            button2.Click += ViewRooms_Click;
            button3.Click += BookNow_Click;
            label7.Click += ViewRooms_Click;
            label6.Click += BookNow_Click;
        }

        private void ViewRooms_Click(object sender, EventArgs e)
        {
            Form2 roomsForm = new Form2();
            this.Hide();
            roomsForm.Show();
            roomsForm.FormClosed += (s, args) => this.Show();
        }

        private void BookNow_Click(object sender, EventArgs e)
        {
            Form3 reservationForm = new Form3();
            this.Hide();
            reservationForm.Show();
            reservationForm.FormClosed += (s, args) => this.Show();
        }

        private void ApplyRoundedCorners()
        {
            int buttonCornerRadius = 18;
            int imageCornerRadius = 16;

            ApplyRoundedRegion(button2, buttonCornerRadius);
            ApplyRoundedRegion(button3, buttonCornerRadius);
            ApplyRoundedRegion(pictureBox1, imageCornerRadius);
        }

        private void ApplyRoundedRegion(Control control, int radius)
        {
            if (control == null) return;
            control.Resize += (sender, e) => UpdateControlRegion(control, radius);
            UpdateControlRegion(control, radius);
        }

        private void UpdateControlRegion(Control control, int radius)
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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            Application.Exit();
        }
    }

    public class AdminLoginForm : Form
    {
        private const string AdminUsername = "admin";
        private const string AdminPassword = "admin123";
        private readonly TextBox txtUsername;
        private readonly TextBox txtPassword;

        public AdminLoginForm()
        {
            Text = "Administrator Login";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(420, 300);
            BackColor = Color.FromArgb(247, 243, 240);
            Font = new Font("Segoe UI", 9.5f);

            Label title = new Label { Text = "Admin Login", Font = new Font("Georgia", 20f, FontStyle.Bold | FontStyle.Italic), ForeColor = Color.FromArgb(61, 42, 48), AutoSize = true, Location = new Point(115, 28) };
            Label subtitle = new Label { Text = "Sign in to open the dashboard", ForeColor = Color.FromArgb(100, 85, 88), AutoSize = true, Location = new Point(112, 70) };
            Label usernameLabel = new Label { Text = "Username", Font = new Font("Georgia", 9.5f, FontStyle.Bold), ForeColor = Color.FromArgb(61, 42, 48), AutoSize = true, Location = new Point(45, 112) };
            txtUsername = new TextBox { Location = new Point(150, 108), Size = new Size(220, 26), TabIndex = 0 };
            Label passwordLabel = new Label { Text = "Password", Font = new Font("Georgia", 9.5f, FontStyle.Bold), ForeColor = Color.FromArgb(61, 42, 48), AutoSize = true, Location = new Point(45, 153) };
            txtPassword = new TextBox { Location = new Point(150, 149), Size = new Size(220, 26), UseSystemPasswordChar = true, TabIndex = 1 };

            Button btnLogin = new Button { Text = "Login", Location = new Point(150, 205), Size = new Size(105, 38), BackColor = Color.FromArgb(122, 31, 62), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Georgia", 10f, FontStyle.Bold), Cursor = Cursors.Hand, TabIndex = 2 };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += Login_Click;
            Button btnCancel = new Button { Text = "Cancel", Location = new Point(265, 205), Size = new Size(105, 38), BackColor = Color.White, ForeColor = Color.FromArgb(61, 42, 48), FlatStyle = FlatStyle.Flat, Font = new Font("Georgia", 10f, FontStyle.Bold), Cursor = Cursors.Hand, DialogResult = DialogResult.Cancel, TabIndex = 3 };
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(185, 123, 46);

            Controls.AddRange(new Control[] { title, subtitle, usernameLabel, txtUsername, passwordLabel, txtPassword, btnLogin, btnCancel });
            AcceptButton = btnLogin;
            CancelButton = btnCancel;
        }

        private void Login_Click(object sender, EventArgs e)
        {
            if (string.Equals(txtUsername.Text.Trim(), AdminUsername, StringComparison.OrdinalIgnoreCase) && txtPassword.Text == AdminPassword)
            {
                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            MessageBox.Show("Incorrect username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtPassword.Clear();
            txtPassword.Focus();
        }

        public static void OpenDashboard(Form owner)
        {
            using (AdminLoginForm login = new AdminLoginForm())
            {
                if (login.ShowDialog(owner) != DialogResult.OK)
                    return;
            }

            Form4 dashboard = new Form4();
            owner.Hide();
            dashboard.FormClosed += (sender, args) => owner.Show();
            dashboard.Show();
        }
    }
}
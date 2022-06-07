namespace KeyPadMaster
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int index = -1;
        Color color;
        
        List<ButtonProperties> buttons = new(16);
        

        private void clearColor()
        {
            foreach(Button btn in panel1.Controls)
            {
                btn.BackColor = saveBTN.BackColor;
                btn.ForeColor = saveBTN.ForeColor;
            }
            SELbtn.BackColor = Color.FromArgb(255, 255, 192);
            L1button.BackColor = Color.FromArgb(128, 128, 255);
            L2button.BackColor = Color.FromArgb(128, 128, 255);
            L3button.BackColor = Color.FromArgb(128, 128, 255);
        }

        private void button_Click(object sender, EventArgs e)
        {
            int btnNo = -1;
            if (sender == null) return;
            int.TryParse((string?)(sender as Button).Tag, out btnNo);

            index = btnNo;
            clearColor();
            (sender as Button).BackColor = Color.FromArgb(255,150,220);
        }

        private void textBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            colorDialog1 = new ColorDialog();
            colorDialog1.ShowDialog();

            color = colorDialog1.Color;
            
            textBox1.Text = $"({color.R}, {color.G}, {color.B})";
        }

        private void saveBTN_Click(object sender, EventArgs e)
        {

        }
    }

    public class ButtonProperties
    {
        protected int index = -1;
        bool enable = false;
        public Color color;
        private IBtnAction action;

        public ButtonProperties(int idx)  
        {
            index = idx; 
            action = new NoAction();
        }
        
        public string GetColorCode()
        {
            return $"{index}: ({color.R}, {color.G}, {color.B})";
        }

        public string GetActionCode()
        {
            if (!enable)
            {
                return "";
            }
            else
            {
                return $"k == {index}:{Environment.NewLine}    {action.GenerateCode()}";
            }
        }

    }

    class NoAction : IBtnAction
    {
        public string GenerateCode()
        {
            return "";
        }
    }

    public interface IBtnAction 
    {
        public string GenerateCode();
    }
}
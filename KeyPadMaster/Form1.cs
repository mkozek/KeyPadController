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
        Keys shortcutSelectedKey = Keys.None;

        Dictionary<int,ButtonProperties> buttons = new(16);
        

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

        private void ElementSelected(object sender, EventArgs e)
        {
            int btnNo = -1;
            if (sender == null) return;
            _ = int.TryParse((string?)((Button)sender).Tag, out btnNo);

            index = btnNo;
            clearColor();
            ((Button)sender).BackColor = Color.FromArgb(255,150,220);
        }

        private void ColorPickerTextBox_DoubleClick(object sender, MouseEventArgs e)
        {
            colorDialog1 = new ColorDialog();
            colorDialog1.ShowDialog();

            color = colorDialog1.Color;
            
            textBox1.Text = $"({color.R}, {color.G}, {color.B})";
        }

        private void SaveBTN_Click(object sender, EventArgs e)
        {

        }

        private void ShortcutKeyTxtBox_Click(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            KeyDown += Form1_KeyDown;
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            var a = e.KeyCode;
            shortcutSelectedKey = a;
            textBox3.Text = a switch
            {
                Keys.F1 => "Keycode.F1",
                _ => a.ToString().ToUpper(),
            };
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
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
        public void ChangeAction(IBtnAction newAction)
        {
            action = newAction;
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

    static class Functions
    {
        public static string GetKeyCode(Keys key)
        {
            return(key.ToString());
        }
    }

    class ShortcutAction : IBtnAction
    {
        bool isAlt = false;
        bool isShift = false;
        bool isWindows = false;
        bool isControl = false;
        Keys key;

        public ShortcutAction(Keys key, bool isAlt = false, bool isShift = false, bool isWindows = false, bool isControl = false)
        {
            this.key = key;
            this.isAlt = isAlt;
            this.isShift = isShift;   
            this.isWindows = isWindows;
            this.isControl = isControl;

        }

        public string GenerateCode()
        {
            return($"keyboard.send({(isAlt)?("keycode.ALT, "):("")}
                {(isShift)?("keycode.SHIFT, "):("")}
                {(isControl)?("keycode.CONTROL, "):("")}
                {(isWindows)?("keycode.GUI, "):("")}
                keycode.{Functions.GetKeyCode(key)})");
        }
    }

    class TextAction : IBtnAction
    {
        string text;

        public TextAction(string text)
        {
            this.text = text;
        }

        public string GenerateCode()
        {
            return($"layout.write(\"{text}\")");
        }
    }

    class MediaAction : IBtnAction
    {
        string media;

        public MediaAction(string selectedMediaAction)
        {
            media = selectedMediaAction;
        }

        public string GenerateCode()
        {
            return($"ConsumerControl.send(ConsumerControlCode.{media})");
        }
    }

    public interface IBtnAction 
    {
        public string GenerateCode();
    }
}
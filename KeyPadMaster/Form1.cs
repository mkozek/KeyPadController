namespace KeyPadMaster
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            controllableBtns.Add(4,button4);
            controllableBtns.Add(5,button5);
            controllableBtns.Add(6,button9);
            controllableBtns.Add(7,button13);
            controllableBtns.Add(8, button2);
            controllableBtns.Add(9, button7);
            controllableBtns.Add(10, button11);
            controllableBtns.Add(11, button23);
            controllableBtns.Add(12, button3);
            controllableBtns.Add(13, button6);
            controllableBtns.Add(14, button10);
            controllableBtns.Add(15, button14);
            allButtons.Add(1, buttonsL1);
            allButtons.Add(2, buttonsL2);
            allButtons.Add(3, buttonsL3);
        }
        int index = -1;
        int selectedLayer = 1;
        Color color;
        Keys shortcutSelectedKey = Keys.None;
        Dictionary<int, ButtonProperties> buttonsL1 = new(16);
        Dictionary<int, ButtonProperties> buttonsL2 = new(16);
        Dictionary<int, ButtonProperties> buttonsL3 = new(16);
        readonly List<string> actionImports = new()
        {
            "import usb_hid",
            "from adafruit_hid.keyboard import Keyboard",
            "from adafruit_hid.keyboard_layout_us import KeyboardLayoutUS",
            "from adafruit_hid.keycode import Keycode",
            "from adafruit_hid.consumer_control import ConsumerControl",
            "from adafruit_hid.consumer_control_code import ConsumerControlCode",
            "keyboard = Keyboard(usb_hid.devices)",
            "layout = KeyboardLayoutUS(keyboard)",
            "consumer_control = ConsumerControl(usb_hid.devices)",
            "",
            ""
        };
        readonly List<string> emptyLines = new()
        {
            "",
            "",
            ""
        };
        Dictionary<int,Button> controllableBtns = new();
        Dictionary<int, Dictionary<int, ButtonProperties>> allButtons = new();

        private void ClearColor()
        {
            foreach(Button btn in panel1.Controls)
            {
                btn.BackColor = saveBTN.BackColor;
                btn.ForeColor = saveBTN.ForeColor;
            }
            foreach(var keyBtn in controllableBtns)
            {
                if (allButtons.ContainsKey(selectedLayer))
                {
                    var layer = allButtons[selectedLayer];
                    if (layer.ContainsKey(keyBtn.Key))
                    {
                        (keyBtn.Value).BackColor = layer.GetValueOrDefault(keyBtn.Key).color;
                    }
                     
                }
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
            ClearColor();
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
            IBtnAction? action = null;
            ButtonProperties props;

            if (MediaActionRBTN.Checked)
            {
                action = new MediaAction(selMediaActionCombo.SelectedItem.ToString());
            }
            if (TextActionRBTN.Checked)
            {
                action = new TextAction(textActionTxtBox.Text);
            }
            if (ShortcutActionRBTN.Checked)
            {
                action = new ShortcutAction(shortcutSelectedKey, ALTcb.Checked, SHIFTcb.Checked, WINDOWScb.Checked, CTRLcb.Checked);
            }
            if(action != null)
            {
                props = new ButtonProperties(index, action, color,EnableActionCheckBox.Checked);

            }
            else
            {
                props = new ButtonProperties(index);
            }



            if (radioButtonLayer1.Checked)
            {
                if (!buttonsL1.TryAdd(index, props))
                {
                    buttonsL1.Remove(index);
                    buttonsL1.Add(index, props);
                } 
            }
            else if (radioButtonLayer2.Checked)
            {
                if (!buttonsL2.TryAdd(index, props))
                {
                    buttonsL2.Remove(index);
                    buttonsL2.Add(index, props);
                }
            }
            else if (radioButtonLayer3.Checked)
            {
                if (!buttonsL3.TryAdd(index, props))
                {
                    buttonsL3.Remove(index);
                    buttonsL3.Add(index, props);
                }
            }
            else
            {
                throw new NullReferenceException("No Layer Selected");
            }
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
            shortcutKeyCode.Text = a switch
            {
                Keys.F1 => "Keycode.F1",
                _ => a.ToString().ToUpper(),
            };
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void SaveLayer_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            string path = folderBrowserDialog1.SelectedPath;
            string ActFile = selectedLayer switch
            {
                1 => "\\Layer1Action.py",
                2 => "\\Layer2Action.py",
                3 => "\\Layer3Action.py",
                _ => "\\Layer1Action.py"
            };
            string ColorFile = selectedLayer switch
            {
                1 => "\\Layer1Color.py",
                2 => "\\Layer2Color.py",
                3 => "\\Layer3Color.py",
                _ => "\\Layer1Color.py"
            };

            if (File.Exists(path + ActFile))
            {
                File.Delete(path + ActFile);
            }
            if (File.Exists(path + ColorFile))
            {
                File.Delete(path + ColorFile);
            }
            
            // Actions
            int index = 0;
            foreach (ButtonProperties key in buttonsL1.Values)
            {
                if(index == 0)
                {
                    File.AppendAllLines(path + ActFile, actionImports);
                    File.AppendAllText(path + ActFile, "def Layer1(k)"+ Environment.NewLine);
                    File.AppendAllText(path + ActFile, Functions.GetIdent(1)+"if ");
                    index = 1;
                }
                else
                {
                    File.AppendAllText(path + ActFile, Functions.GetIdent(1) + "elif ");
                }

                File.AppendAllText(path + ActFile, key.GetActionCode(2) + Environment.NewLine);

            }
            File.AppendAllLines(path + ActFile, emptyLines);
            index = 0;

            // Colors
            foreach (ButtonProperties key in buttonsL1.Values)
            {
                if (index == 0)
                {
                    File.AppendAllText(path + ColorFile, "Color1 = {");
                    index = 1;
                }
                else
                {
                    File.AppendAllText(path + ColorFile, "," + Environment.NewLine + Functions.GetIdent(3));
                }
                File.AppendAllText(path + ColorFile, key.GetColorCode());
            }
            File.AppendAllText(path + ColorFile, "}");
            File.AppendAllLines(path + ColorFile, emptyLines);
        }

        private void RadioButtonLayer_CheckedChanged(object sender, EventArgs e)
        {
            var sender1 = sender as RadioButton;
            if (sender1 != null) 
            selectedLayer = (sender1).Text switch
            {
                "Layer 1" => 1,
                "Layer 2" => 2,
                "Layer 3" => 3,
                _ => 1
            };
            ClearColor();
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

        public ButtonProperties(int idx, IBtnAction btnAction,Color color,bool EnableAction)
        {
            action = btnAction;
            index = idx;
            this.color = color;
            enable = EnableAction;
        }


        public void ChangeAction(IBtnAction newAction)
        {
            action = newAction;
        }
        
        public string GetColorCode()
        {
            return $"{index}: ({color.R}, {color.G}, {color.B})";
        }
        public string GetColorCode(int Ident)
        {
            return $"{Functions.GetIdent(Ident)}{index}: ({color.R}, {color.G}, {color.B})";
        }

        public string GetActionCode()
        {
            if (!enable)
            {
                return "";
            }
            else
            {
                return $"k == {index}:{Environment.NewLine}{Functions.GetIdent(1)}{action.GenerateCode()}";
            }
        }
        public string GetActionCode(int Ident)
        {
            if (!enable)
            {
                return "";
            }
            else
            {
                return $"k == {index}:{Environment.NewLine}{Functions.GetIdent(Ident)}{action.GenerateCode()}";
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

        public static string GetIdent(int number)
        {
            string ident = "    ";
            string ret = "";
            for (int i = 0; i < number; i++)
            {
                ret += ident;
            }
            return ret;
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
            string v = $"keyboard.send({(isAlt ? ("keycode.ALT, ") : (""))}";
            v += $"{(isShift ? ("keycode.SHIFT, ") : (""))}";
            v += $"{(isControl ? ("keycode.CONTROL, ") : (""))}";
            v += $"{(isWindows ? ("keycode.GUI, ") : (""))}";
            v+= $"keycode.{Functions.GetKeyCode(key)})";
            return v;
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
            return($"consumer_control.send(ConsumerControlCode.{media})");
        }
    }

    public interface IBtnAction 
    {
        public string GenerateCode();
    }
}

using System.Diagnostics;
namespace EZSens {
public partial class Form1 : Form {
    Label                      gamelistLabel, aratioLabel, mdratioLabel, hipfovLabel, hipdistLabel, go;
    LinkLabel                  url;
    ComboBox                   gamelistBox, aratioBox;
    TextBox                    mdratioBox, hipfovBox, hipdistBox, result;
    Button                     Calc;
    int                        gameidx, aridx;
    bool                       suutihantei;
    double                     aratio, mdratio, hipfov, hipdist, alpha0, alpha1;
    Dictionary<string, double> fovWithOptics, distanceWithOptics;
    public Form1() {
        InitializeComponent();
        Width           = 600;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox     = false;

        gamelistLabel          = new Label();
        gamelistLabel.AutoSize = true;
        gamelistLabel.Location = new Point (20, 20);
        gamelistLabel.Text     = "・Game title";

        aratioLabel          = new Label();
        aratioLabel.AutoSize = true;
        aratioLabel.Location = new Point (20, 70);
        aratioLabel.Text     = "・Aspact ratio";

        mdratioLabel          = new Label();
        mdratioLabel.AutoSize = true;
        mdratioLabel.Location = new Point (20, 120);
        mdratioLabel.Text     = "・Monitor Distance [%]";

        hipfovLabel          = new Label();
        hipfovLabel.AutoSize = true;
        hipfovLabel.Location = new Point (20, 170);
        hipfovLabel.Text     = "・In-Game Hipfire FOV";

        hipdistLabel          = new Label();
        hipdistLabel.AutoSize = true;
        hipdistLabel.Location = new Point (20, 220);
        hipdistLabel.Text     = "・Hipfire 360° Distance [cm]";

        go          = new Label();
        go.AutoSize = true;
        go.Location = new Point (200, 380);

        url          = new LinkLabel();
        url.AutoSize = true;
        url.Location = new Point (220, 380);
        url.Click += new EventHandler (urlClick!);

        result           = new TextBox();
        result.Size      = new Size (360, 350);
        result.Multiline = true;
        // result.ReadOnly = true;
        result.Location = new Point (180, 20);
        result.Text     = "計算結果";

        string[] gamelist    = new string[] { "Apex", "R6S", "Valorant", "Splitgate", "CSGO", "Overwatch" };
        gamelistBox          = new ComboBox();
        gamelistBox.Location = new Point (20, 40);
        gamelistBox.Items.AddRange (gamelist);
        gamelistBox.DropDownStyle = ComboBoxStyle.DropDownList; // 入力不可のリストにする
        gamelistBox.SelectedIndexChanged += new EventHandler (gameSelected!);

        string[] aratiolist = new string[] { "16:9", "5:3", "16:10", "3:2", "4:3", "5:4", "1:1" };
        aratioBox           = new ComboBox();
        aratioBox.Location  = new Point (20, 90);
        aratioBox.Items.AddRange (aratiolist);
        aratioBox.DropDownStyle = ComboBoxStyle.DropDownList;

        mdratioBox          = new TextBox();
        mdratioBox.Location = new Point (20, 140);

        hipfovBox          = new TextBox();
        hipfovBox.Location = new Point (20, 190);

        hipdistBox          = new TextBox();
        hipdistBox.Location = new Point (20, 240);

        Calc               = new Button();
        Calc.Location      = new Point (460, 405);
        Calc.Name          = "計算実行";
        Calc.Size          = new Size (75, 23);
        Calc.TabIndex      = 0;
        Calc.Text          = "計算実行";
        fovWithOptics      = new Dictionary<string, double>();
        distanceWithOptics = new Dictionary<string, double>();
        Calc.Click += new EventHandler (Calc_Click!);

        Controls.Add (gamelistBox);
        Controls.Add (gamelistLabel);
        Controls.Add (aratioBox);
        Controls.Add (aratioLabel);
        Controls.Add (mdratioBox);
        Controls.Add (mdratioLabel);
        Controls.Add (go);
        Controls.Add (url);
        Controls.Add (hipfovBox);
        Controls.Add (hipfovLabel);
        Controls.Add (hipdistBox);
        Controls.Add (hipdistLabel);
        Controls.Add (Calc);
        Controls.Add (result);
        Text = "EZSens C#";
    }
    private double division (string frac) {
        string[] arr = frac.Split ('/');
        return float.Parse (arr[0]) / float.Parse (arr[1]);
    }
    private string setPrecision (double value, int digits) {
        string ret = value.ToString();
        if (ret.Length > digits + 1)
            ret = Math.Round (value, digits + (value < 1 ? 1 : 0) - ret.IndexOf ('.')).ToString();
        return ret;
    }
    private void urlClick (object sender, EventArgs e) {
        url.LinkVisited = true;
        var info        = new ProcessStartInfo {
            UseShellExecute = true,
            FileName        = "https://www.youtube.com/watch?v=n5xQiK14soE",
        };
        Process.Start (info);
    }
    private void gameSelected (object sender, EventArgs e) {
        gameidx = gamelistBox.SelectedIndex;
        switch (gameidx) {
        case 2:
            aratioBox.SelectedIndex = 0;
            aratioBox.Enabled       = false;
            hipfovBox.Text          = "103";
            hipfovBox.ReadOnly      = true;
            break;
        case 5:
            aratioBox.SelectedIndex = 0;
            hipfovBox.Text          = "";
            aratioBox.Enabled       = false;
            hipfovBox.ReadOnly      = false;
            break;
        default:
            aratioBox.SelectedIndex = -1;
            hipfovBox.Text          = "";
            aratioBox.Enabled       = true;
            hipfovBox.ReadOnly      = false;
            break;
        }
    }
    void VtoH() {
        foreach (string key in fovWithOptics.Keys) {
            fovWithOptics[key] =
              2 * Math.Atan (Math.Tan (fovWithOptics[key] * hipfov / 2 * Math.PI / 180) * aratio) / Math.PI * 180;
        }
        hipfov = 2 * Math.Atan (Math.Tan (hipfov / 2 * Math.PI / 180) * aratio) / Math.PI * 180;
    }
    void Hdeg4to3Fix() {
        foreach (string key in fovWithOptics.Keys) {
            fovWithOptics[key] =
              2 * Math.Atan (Math.Tan (fovWithOptics[key] * hipfov / 2 * Math.PI / 180) * aratio * 3 / 4) / Math.PI *
              180;
        }
        hipfov = 2 * Math.Atan (Math.Tan (hipfov / 2 * Math.PI / 180) * aratio * 3 / 4) / Math.PI * 180;
    }
    void calcSens() {
        foreach (string key in distanceWithOptics.Keys) {
            alpha0                  = Math.Atan (mdratio * Math.Tan (hipfov / 2 * Math.PI / 180));
            alpha1                  = Math.Atan (mdratio * Math.Tan (fovWithOptics[key] / 2 * Math.PI / 180));
            distanceWithOptics[key] = hipdist * alpha0 / alpha1;
        }
    }
    private void Calc_Click (object sender, EventArgs e) {
        // 入力が正しいか判定
        gameidx     = gamelistBox.SelectedIndex;
        aridx       = aratioBox.SelectedIndex;
        suutihantei = (double.TryParse (mdratioBox.Text, out aratio) && double.TryParse (hipfovBox.Text, out hipfov) &&
                       double.TryParse (hipdistBox.Text, out hipdist));
        if (gameidx == -1 || aridx == -1 || !suutihantei) {
            MessageBox.Show ("値をすべて入力してください。選択以外は数値で入力してください。", "エラー",
                             MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。->
                               // 先にインデックスで判定してるからnullは来ない
        aratio = division (aratioBox.SelectedItem.ToString().Replace (":", "/"));
#pragma warning restore CS8602 // null 参照の可能性があるものの逆参照です。
        mdratio = double.Parse (mdratioBox.Text) / 100;
        if (mdratio > 100 || mdratio < 0) {
            MessageBox.Show ("Monitar Distanceは0から100の割合で入力してください。", "エラー", MessageBoxButtons.OK,
                             MessageBoxIcon.Error);
            return;
        }
        if (mdratio == 0) { mdratio = 0.00000000000000000000000000000001; }
        hipfov  = double.Parse (hipfovBox.Text);
        hipdist = double.Parse (hipdistBox.Text);
        if (hipdist < 0) {
            MessageBox.Show ("振り向きは自然数で入力してください。", "エラー", MessageBoxButtons.OK,
                             MessageBoxIcon.Error);
            return;
        }
        switch (gameidx) {
        case 0:
            if (hipfov < 1 || hipfov > 2) {
                MessageBox.Show ("In-Game FOVは1から2のfovScaleで入力してください。", "エラー", MessageBoxButtons.OK,
                                 MessageBoxIcon.Error);
                return;
            }
            fovWithOptics.Clear();
            fovWithOptics.Add ("1x, Pistol, SMG, SG", 6.0 / 7.0);
            fovWithOptics.Add ("AR, LMG Sniper", 11.0 / 14.0);
            fovWithOptics.Add ("2x", 1102591.0 / 2000000.0);
            fovWithOptics.Add ("3x", 26275563.0 / 70000000.0);
            fovWithOptics.Add ("4x", 992913.0 / 3500000.0);
            fovWithOptics.Add ("6x", 1901823.0 / 10000000.0);
            fovWithOptics.Add ("8x", 1.0 / 7.0);
            fovWithOptics.Add ("10x", 1001339.0 / 8750000.0);

            distanceWithOptics.Clear();
            distanceWithOptics.Add ("1x, Pistol, SMG, SG", 0.0);
            distanceWithOptics.Add ("AR, LMG Sniper", 0.0);
            distanceWithOptics.Add ("2x", 0.0);
            distanceWithOptics.Add ("3x", 0.0);
            distanceWithOptics.Add ("4x", 0.0);
            distanceWithOptics.Add ("6x", 0.0);
            distanceWithOptics.Add ("8x", 0.0);
            distanceWithOptics.Add ("10x", 0.0);

            hipfov *= 70;
            Hdeg4to3Fix();
            calcSens();
            break;
        case 1:
            if (hipfov < 60 || hipfov > 90) {
                MessageBox.Show ("In-Game FOVは60から90で入力してください。", "エラー", MessageBoxButtons.OK,
                                 MessageBoxIcon.Error);
                return;
            }
            fovWithOptics.Clear();
            fovWithOptics.Add ("1x", 0.9);
            fovWithOptics.Add ("1.5x", 0.59);
            fovWithOptics.Add ("2x", 0.49);
            fovWithOptics.Add ("2.5x", 0.42);
            fovWithOptics.Add ("3x", 0.35);
            fovWithOptics.Add ("4x", 0.3);
            fovWithOptics.Add ("5x", 0.22);
            fovWithOptics.Add ("12x", 0.092);

            distanceWithOptics.Clear();
            distanceWithOptics.Add ("1x", 0.0);
            distanceWithOptics.Add ("1.5x", 0.0);
            distanceWithOptics.Add ("2x", 0.0);
            distanceWithOptics.Add ("2.5x", 0.0);
            distanceWithOptics.Add ("3x", 0.0);
            distanceWithOptics.Add ("4x", 0.0);
            distanceWithOptics.Add ("5x", 0.0);
            distanceWithOptics.Add ("12x", 0.0);

            VtoH();
            calcSens();
            break;
        case 2:
            fovWithOptics.Clear();
            fovWithOptics.Add ("1.15x - Ares, Odin, Spectre, Stinger", hipfov / 1.15);
            fovWithOptics.Add ("1.25x - Bulldog, Phantom, Vandal", hipfov / 1.25);
            fovWithOptics.Add ("1.50x - Guardian, Headhunter", hipfov / 1.5);
            fovWithOptics.Add ("2.50x - Operator, Tour de Force", hipfov / 2.5);
            fovWithOptics.Add ("3.50x - Marshal", hipfov / 3.5);
            fovWithOptics.Add ("5x - Operator", hipfov / 5.0);

            distanceWithOptics.Clear();
            distanceWithOptics.Add ("1.15x - Ares, Odin, Spectre, Stinger", 0.0);
            distanceWithOptics.Add ("1.25x - Bulldog, Phantom, Vandal", 0.0);
            distanceWithOptics.Add ("1.50x - Guardian, Headhunter", 0.0);
            distanceWithOptics.Add ("2.50x - Operator, Tour de Force", 0.0);
            distanceWithOptics.Add ("3.50x - Marshal", 0.0);
            distanceWithOptics.Add ("5x - Operator", 0.0);

            calcSens();
            break;
        case 3:
            if (hipfov < 0 || hipfov > 180) {
                MessageBox.Show ("In-Game FOVは180未満の自然数で入力してください。", "エラー", MessageBoxButtons.OK,
                                 MessageBoxIcon.Error);
                return;
            }
            fovWithOptics.Clear();
            fovWithOptics.Add ("Assault Rifle, Carbine, Rocket", 55.0);
            fovWithOptics.Add ("Battle Rifle, Railgun", 40.0);
            fovWithOptics.Add ("Sniper Rifle", 30.0);

            distanceWithOptics.Clear();
            distanceWithOptics.Add ("Assault Rifle, Carbine, Rocket", 0.0);
            distanceWithOptics.Add ("Battle Rifle, Railgun", 0.0);
            distanceWithOptics.Add ("Sniper Rifle", 0.0);

            if (aridx != 0) {
                hipfov = 2 * Math.Atan (Math.Tan (hipfov / 2 * Math.PI / 180) * aratio * 9 / 16) / Math.PI * 180;
                foreach (string key in fovWithOptics.Keys) {
                    fovWithOptics[key] =
                      2 * Math.Atan (Math.Tan (fovWithOptics[key] / 2 * Math.PI / 180) * aratio * 9 / 16) / Math.PI *
                      180;
                }
            }
            calcSens();
            break;
        case 4:
            fovWithOptics.Clear();
            fovWithOptics.Add ("Zoomed 1: AWP, SSG 08, G3SG1, SCAR-20", 4.0 / 9.0);
            fovWithOptics.Add ("Zoomed 2: AWP", 1.0 / 9.0);
            fovWithOptics.Add ("Zoomed 2: SSG 08, G3SG1, SCAR-20", 15.0 / 90.0);
            fovWithOptics.Add ("Zoomed: AUG, SG 553", 0.5);

            distanceWithOptics.Clear();
            distanceWithOptics.Add ("Zoomed 1: AWP, SSG 08, G3SG1, SCAR-20", 0.0);
            distanceWithOptics.Add ("Zoomed 2: AWP", 0.0);
            distanceWithOptics.Add ("Zoomed 2: SSG 08, G3SG1, SCAR-20", 0.0);
            distanceWithOptics.Add ("Zoomed: AUG, SG 553", 0.0);

            Hdeg4to3Fix();
            calcSens();
            break;
        case 5:
            if (hipfov < 80 || hipfov > 103) {
                MessageBox.Show ("In-Game FOVは80から103で入力してください。", "エラー", MessageBoxButtons.OK,
                                 MessageBoxIcon.Error);
                return;
            }
            fovWithOptics.Clear();
            fovWithOptics.Add ("Ashe ADS", 40.0);
            fovWithOptics.Add ("Widowmaker/Ana Scope", 30.0);

            distanceWithOptics.Clear();
            distanceWithOptics.Add ("1x, Pistol, SMG, SG", 0.0);
            distanceWithOptics.Add ("AR, LMG Sniper", 0.0);

            calcSens();
            break;
        }
        result.Text = "計算結果\r\n";
        foreach (KeyValuePair<string, double> item in distanceWithOptics) {
            result.AppendText ("\r\n" + item.Key + " は " + setPrecision (item.Value, 8) + "[cm/360°]\r\n");
        }
        go.Text  = "go";
        url.Text = "https://www.mouse-sensitivity.com/";
    }
}
}
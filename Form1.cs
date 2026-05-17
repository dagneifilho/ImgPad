namespace ImgPad;

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class Form1 : Form
{
    private TextBox txtPasta;
    private StyledButton btnSelecionar;
    private StyledButton btnIniciar;
    private Label lblTitulo;
    private Label lblSubtitulo;
    private RichTextBox txtLog;
    private ProgressBar progressBar;

    // Paleta
    private readonly Color BG      = Color.FromArgb(18, 18, 20);
    private readonly Color SURFACE = Color.FromArgb(28, 28, 32);
    private readonly Color BORDER  = Color.FromArgb(50, 50, 58);
    private readonly Color ACCENT  = Color.FromArgb(99, 102, 241);
    private readonly Color TEXT    = Color.FromArgb(240, 240, 245);
    private readonly Color MUTED   = Color.FromArgb(120, 120, 135);

    public Form1()
    {
        this.Text = "ImgPad";
        this.Size = new Size(520, 440);
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = BG;

        // Título
        lblTitulo = new Label();
        lblTitulo.Text = "ImgPad";
        lblTitulo.Font = new Font("Segoe UI", 16f, FontStyle.Bold);
        lblTitulo.ForeColor = TEXT;
        lblTitulo.Location = new Point(24, 24);
        lblTitulo.AutoSize = true;

        // Subtítulo
        lblSubtitulo = new Label();
        lblSubtitulo.Text = "Padronização de imagens para e-commerce";
        lblSubtitulo.Font = new Font("Segoe UI", 9f);
        lblSubtitulo.ForeColor = MUTED;
        lblSubtitulo.Location = new Point(26, 52);
        lblSubtitulo.AutoSize = true;

        // Separador
        var linha = new Panel();
        linha.BackColor = BORDER;
        linha.Location = new Point(24, 82);
        linha.Size = new Size(460, 1);

        // Label pasta
        var lblPasta = new Label();
        lblPasta.Text = "PASTA DE ORIGEM";
        lblPasta.Font = new Font("Segoe UI", 7.5f, FontStyle.Bold);
        lblPasta.ForeColor = MUTED;
        lblPasta.Location = new Point(24, 100);
        lblPasta.AutoSize = true;

        // Campo da pasta
        txtPasta = new TextBox();
        txtPasta.PlaceholderText = "Nenhuma pasta selecionada...";
        txtPasta.ReadOnly = true;
        txtPasta.Location = new Point(24, 118);
        txtPasta.Size = new Size(330, 32);
        txtPasta.Font = new Font("Segoe UI", 9.5f);
        txtPasta.BackColor = SURFACE;
        txtPasta.ForeColor = TEXT;
        txtPasta.BorderStyle = BorderStyle.FixedSingle;

        // Botão selecionar
        btnSelecionar = new StyledButton();
        btnSelecionar.Text = "Selecionar";
        btnSelecionar.Location = new Point(364, 117);
        btnSelecionar.Size = new Size(120, 32);
        btnSelecionar.NormalColor = SURFACE;
        btnSelecionar.HoverColor = Color.FromArgb(45, 45, 55);
        btnSelecionar.BorderColor = BORDER;
        btnSelecionar.TextColor = TEXT;
        btnSelecionar.Click += BtnSelecionar_Click;

        // Botão iniciar
        btnIniciar = new StyledButton();
        btnIniciar.Text = "Iniciar";
        btnIniciar.Location = new Point(364, 160);
        btnIniciar.Size = new Size(120, 36);
        btnIniciar.NormalColor = ACCENT;
        btnIniciar.HoverColor = Color.FromArgb(79, 82, 210);
        btnIniciar.BorderColor = ACCENT;
        btnIniciar.TextColor = Color.White;
        btnIniciar.Enabled = false;
        btnIniciar.Click += BtnIniciar_Click;

        // Barra de progresso
        progressBar = new ProgressBar();
        progressBar.Location = new Point(24, 210);
        progressBar.Size = new Size(460, 6);
        progressBar.Style = ProgressBarStyle.Continuous;
        progressBar.BackColor = SURFACE;
        progressBar.ForeColor = ACCENT;

        // Separador 2
        var linha2 = new Panel();
        linha2.BackColor = BORDER;
        linha2.Location = new Point(24, 228);
        linha2.Size = new Size(460, 1);

        // Log
        txtLog = new RichTextBox();
        txtLog.Location = new Point(24, 240);
        txtLog.Size = new Size(460, 155);
        txtLog.BackColor = Color.FromArgb(12, 12, 14);
        txtLog.ForeColor = Color.FromArgb(180, 180, 190);
        txtLog.Font = new Font("Cascadia Code", 8.5f);
        txtLog.ReadOnly = true;
        txtLog.BorderStyle = BorderStyle.None;
        txtLog.ScrollBars = RichTextBoxScrollBars.Vertical;

        this.Controls.AddRange(new Control[]
        {
            lblTitulo, lblSubtitulo, linha,
            lblPasta, txtPasta,
            btnSelecionar, btnIniciar,
            progressBar, linha2, txtLog
        });
    }

    public void Log(string mensagem, Color? cor = null)
    {
        if (InvokeRequired) { Invoke(() => Log(mensagem, cor)); return; }

        txtLog.SelectionStart = txtLog.TextLength;
        txtLog.SelectionLength = 0;
        txtLog.SelectionColor = cor ?? Color.FromArgb(180, 180, 190);
        txtLog.AppendText(mensagem + "\n");
        txtLog.ScrollToCaret();
    }

    public void SetProgresso(int atual, int total)
    {
        if (InvokeRequired) { Invoke(() => SetProgresso(atual, total)); return; }

        progressBar.Maximum = total;
        progressBar.Value = atual;
        if (atual < total) progressBar.Value = atual + 1;
        progressBar.Value = atual;
    }

    private void BtnSelecionar_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog();
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            txtPasta.Text = dialog.SelectedPath;
            btnIniciar.Enabled = true;
        }
    }

    private void BtnIniciar_Click(object? sender, EventArgs e)
    {
        btnIniciar.Enabled = false;
        txtLog.Clear();
        progressBar.Value = 0;

        Task.Run(() =>
        {
            ImageHandler.ProcessarPasta(txtPasta.Text, Log, SetProgresso);
            Invoke(() => btnIniciar.Enabled = true);
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            txtPasta?.Dispose();
            btnSelecionar?.Dispose();
            btnIniciar?.Dispose();
            lblTitulo?.Dispose();
            lblSubtitulo?.Dispose();
            txtLog?.Dispose();
            progressBar?.Dispose();
        }
        base.Dispose(disposing);
    }
}

public class StyledButton : Button
{
    public Color NormalColor { get; set; } = Color.FromArgb(40, 40, 50);
    public Color HoverColor  { get; set; } = Color.FromArgb(60, 60, 75);
    public Color BorderColor { get; set; } = Color.FromArgb(70, 70, 85);
    public Color TextColor   { get; set; } = Color.White;

    private bool _hovered = false;

    public StyledButton()
    {
        this.FlatStyle = FlatStyle.Flat;
        this.FlatAppearance.BorderSize = 1;
        this.Cursor = Cursors.Hand;
        this.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        _hovered = true;
        this.Invalidate();
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        _hovered = false;
        this.Invalidate();
        base.OnMouseLeave(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var bg = _hovered ? HoverColor : NormalColor;
        using var brush = new SolidBrush(Enabled ? bg : Color.FromArgb(35, 35, 42));
        g.FillRectangle(brush, ClientRectangle);

        using var pen = new Pen(Enabled ? BorderColor : Color.FromArgb(50, 50, 58), 1);
        g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);

        var textColor = Enabled ? TextColor : Color.FromArgb(80, 80, 90);
        using var textBrush = new SolidBrush(textColor);
        var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        g.DrawString(Text, Font, textBrush, ClientRectangle, sf);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Font?.Dispose();
        }
        base.Dispose(disposing);
    }
}
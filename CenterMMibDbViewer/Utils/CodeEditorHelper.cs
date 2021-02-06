using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ScintillaNET;

namespace CenterMMibDbViewer.Utils
{
	internal class CodeEditorHelper
	{
		private CodeEditorHelper()
        {}

        public static CodeEditorHelper instance { get; } = new CodeEditorHelper();

        private Scintilla _scintilla;
        private Form _frm;

        public void Initialize(Scintilla scintilla, Form frm)
        {
            _scintilla = scintilla;
            _frm = frm;


			// INITIAL VIEW CONFIG
			_scintilla.WrapMode = WrapMode.None;
			_scintilla.IndentationGuides = IndentView.LookBoth;

			// STYLING
			InitColors();
			InitSyntaxColoring();

			// NUMBER MARGIN
			InitNumberMargin();

			// BOOKMARK MARGIN
			InitBookmarkMargin();

			// CODE FOLDING MARGIN
			InitCodeFolding();

			// DRAG DROP
			//InitDragDropFile();

			// DEFAULT FILE
			//LoadDataFromFile("../../MainForm.cs");

			// INIT HOTKEYS
			//InitHotkeys();

		}

		private void InitColors()
		{

			_scintilla.SetSelectionBackColor(true, IntToColor(0x114D9C));

		}

		//private void InitHotkeys()
		//{

		//	// register the hotkeys with the form
		//	HotKeyManager.AddHotKey(_frm, OpenSearch, Keys.F, true);
		//	HotKeyManager.AddHotKey(_frm, OpenFindDialog, Keys.F, true, false, true);
		//	HotKeyManager.AddHotKey(_frm, OpenReplaceDialog, Keys.R, true);
		//	HotKeyManager.AddHotKey(_frm, OpenReplaceDialog, Keys.H, true);
		//	HotKeyManager.AddHotKey(_frm, Uppercase, Keys.U, true);
		//	HotKeyManager.AddHotKey(_frm, Lowercase, Keys.L, true);
		//	HotKeyManager.AddHotKey(_frm, ZoomIn, Keys.Oemplus, true);
		//	HotKeyManager.AddHotKey(_frm, ZoomOut, Keys.OemMinus, true);
		//	HotKeyManager.AddHotKey(_frm, ZoomDefault, Keys.D0, true);
		//	HotKeyManager.AddHotKey(_frm, CloseSearch, Keys.Escape);

		//	// remove conflicting hotkeys from scintilla
		//	_scintilla.ClearCmdKey(Keys.Control | Keys.F);
		//	_scintilla.ClearCmdKey(Keys.Control | Keys.R);
		//	_scintilla.ClearCmdKey(Keys.Control | Keys.H);
		//	_scintilla.ClearCmdKey(Keys.Control | Keys.L);
		//	_scintilla.ClearCmdKey(Keys.Control | Keys.U);

		//}

		private void InitSyntaxColoring()
		{

			// Configure the default style
			_scintilla.StyleResetDefault();
			_scintilla.Styles[Style.Default].Font = "Consolas";
			_scintilla.Styles[Style.Default].Size = 10;
			_scintilla.Styles[Style.Default].BackColor = IntToColor(0x212121);
			_scintilla.Styles[Style.Default].ForeColor = IntToColor(0xFFFFFF);
			_scintilla.StyleClearAll();

			// Configure the CPP (C#) lexer styles
			
			_scintilla.Styles[Style.Cpp.Identifier].ForeColor = IntToColor(0xD0DAE2);
			_scintilla.Styles[Style.Cpp.Comment].ForeColor = IntToColor(0xBD758B);
			_scintilla.Styles[Style.Cpp.CommentLine].ForeColor = IntToColor(0x40BF57);
			_scintilla.Styles[Style.Cpp.CommentDoc].ForeColor = IntToColor(0x2FAE35);
			_scintilla.Styles[Style.Cpp.Number].ForeColor = IntToColor(0xFFFF00);
			_scintilla.Styles[Style.Cpp.String].ForeColor = IntToColor(0xFFFF00);
			_scintilla.Styles[Style.Cpp.Character].ForeColor = IntToColor(0xE95454);
			_scintilla.Styles[Style.Cpp.Preprocessor].ForeColor = IntToColor(0x8AAFEE);
			_scintilla.Styles[Style.Cpp.Operator].ForeColor = IntToColor(0xE0E0E0);
			_scintilla.Styles[Style.Cpp.Regex].ForeColor = IntToColor(0xff00ff);
			_scintilla.Styles[Style.Cpp.CommentLineDoc].ForeColor = IntToColor(0x77A7DB);
			_scintilla.Styles[Style.Cpp.Word].ForeColor = IntToColor(0x48A8EE);
			_scintilla.Styles[Style.Cpp.Word2].ForeColor = IntToColor(0xF98906);
			_scintilla.Styles[Style.Cpp.CommentDocKeyword].ForeColor = IntToColor(0xB3D991);
			_scintilla.Styles[Style.Cpp.CommentDocKeywordError].ForeColor = IntToColor(0xFF0000);
			_scintilla.Styles[Style.Cpp.GlobalClass].ForeColor = IntToColor(0x48A8EE);

			_scintilla.Lexer = Lexer.Xml;

			//_scintilla.SetKeywords(0, "class extends implements import interface new case do while else if for in switch throw get set function var try catch finally while with default break continue delete return each const namespace package include use is as instanceof typeof author copy default deprecated eventType example exampleText exception haxe inheritDoc internal link mtasc mxmlc param private return see serial serialData serialField since throws usage version langversion playerversion productversion dynamic private public partial static intrinsic internal native override protected AS3 final super this arguments null Infinity NaN undefined true false abstract as base bool break by byte case catch char checked class const continue decimal default delegate do double descending explicit event extern else enum false finally fixed float for foreach from goto group if implicit in int interface internal into is lock long new null namespace object operator out override orderby params private protected public readonly ref return switch struct sbyte sealed short sizeof stackalloc static string select this throw true try typeof uint ulong unchecked unsafe ushort using var virtual volatile void while where yield");
			//_scintilla.SetKeywords(1, "void Null ArgumentError arguments Array Boolean Class Date DefinitionError Error EvalError Function int Math Namespace Number Object RangeError ReferenceError RegExp SecurityError String SyntaxError TypeError uint XML XMLList Boolean Byte Char DateTime Decimal Double Int16 Int32 Int64 IntPtr SByte Single UInt16 UInt32 UInt64 UIntPtr Void Path File System Windows Forms ScintillaNET");

		}

		private void OnTextChanged(object sender, EventArgs e)
		{

		}


		#region Numbers, Bookmarks, Code Folding

		/// <summary>
		/// the background color of the text area
		/// </summary>
		private const int BACK_COLOR = 0x2A211C;

		/// <summary>
		/// default text color of the text area
		/// </summary>
		private const int FORE_COLOR = 0xB7B7B7;

		/// <summary>
		/// change this to whatever margin you want the line numbers to show in
		/// </summary>
		private const int NUMBER_MARGIN = 1;

		/// <summary>
		/// change this to whatever margin you want the bookmarks/breakpoints to show in
		/// </summary>
		private const int BOOKMARK_MARGIN = 2;
		private const int BOOKMARK_MARKER = 2;

		/// <summary>
		/// change this to whatever margin you want the code folding tree (+/-) to show in
		/// </summary>
		private const int FOLDING_MARGIN = 3;

		/// <summary>
		/// set this true to show circular buttons for code folding (the [+] and [-] buttons on the margin)
		/// </summary>
		private const bool CODEFOLDING_CIRCULAR = true;

		private void InitNumberMargin()
		{

			_scintilla.Styles[Style.LineNumber].BackColor = IntToColor(BACK_COLOR);
			_scintilla.Styles[Style.LineNumber].ForeColor = IntToColor(FORE_COLOR);
			_scintilla.Styles[Style.IndentGuide].ForeColor = IntToColor(FORE_COLOR);
			_scintilla.Styles[Style.IndentGuide].BackColor = IntToColor(BACK_COLOR);

			var nums = _scintilla.Margins[NUMBER_MARGIN];
			nums.Width = 30;
			nums.Type = MarginType.Number;
			nums.Sensitive = true;
			nums.Mask = 0;

			_scintilla.MarginClick += ScintillaMarginClick;
		}

		private void InitBookmarkMargin()
		{

			//_scintilla.SetFoldMarginColor(true, IntToColor(BACK_COLOR));

			var margin = _scintilla.Margins[BOOKMARK_MARGIN];
			margin.Width = 20;
			margin.Sensitive = true;
			margin.Type = MarginType.Symbol;
			margin.Mask = (1 << BOOKMARK_MARKER);
			//margin.Cursor = MarginCursor.Arrow;

			var marker = _scintilla.Markers[BOOKMARK_MARKER];
			marker.Symbol = MarkerSymbol.Circle;
			marker.SetBackColor(IntToColor(0xFF003B));
			marker.SetForeColor(IntToColor(0x000000));
			marker.SetAlpha(100);

		}

		private void InitCodeFolding()
		{

			_scintilla.SetFoldMarginColor(true, IntToColor(BACK_COLOR));
			_scintilla.SetFoldMarginHighlightColor(true, IntToColor(BACK_COLOR));

			// Enable code folding
			_scintilla.SetProperty("fold", "1");
			_scintilla.SetProperty("fold.compact", "1");

			// Configure a margin to display folding symbols
			_scintilla.Margins[FOLDING_MARGIN].Type = MarginType.Symbol;
			_scintilla.Margins[FOLDING_MARGIN].Mask = Marker.MaskFolders;
			_scintilla.Margins[FOLDING_MARGIN].Sensitive = true;
			_scintilla.Margins[FOLDING_MARGIN].Width = 20;

			// Set colors for all folding markers
			for (int i = 25; i <= 31; i++)
			{
				_scintilla.Markers[i].SetForeColor(IntToColor(BACK_COLOR)); // styles for [+] and [-]
				_scintilla.Markers[i].SetBackColor(IntToColor(FORE_COLOR)); // styles for [+] and [-]
			}

			// Configure folding markers with respective symbols
			_scintilla.Markers[Marker.Folder].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CirclePlus : MarkerSymbol.BoxPlus;
			_scintilla.Markers[Marker.FolderOpen].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CircleMinus : MarkerSymbol.BoxMinus;
			_scintilla.Markers[Marker.FolderEnd].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CirclePlusConnected : MarkerSymbol.BoxPlusConnected;
			_scintilla.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
			_scintilla.Markers[Marker.FolderOpenMid].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CircleMinusConnected : MarkerSymbol.BoxMinusConnected;
			_scintilla.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
			_scintilla.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

			// Enable automatic folding
			_scintilla.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);

		}

		private void ScintillaMarginClick(object sender, MarginClickEventArgs e)
		{
			if (e.Margin == BOOKMARK_MARGIN)
			{
				// Do we have a marker for this line?
				const uint mask = (1 << BOOKMARK_MARKER);
				var line = _scintilla.Lines[_scintilla.LineFromPosition(e.Position)];
				if ((line.MarkerGet() & mask) > 0)
				{
					// Remove existing bookmark
					line.MarkerDelete(BOOKMARK_MARKER);
				}
				else
				{
					// Add bookmark
					line.MarkerAdd(BOOKMARK_MARKER);
				}
			}
		}

		#endregion

		#region Drag & Drop File

		//public void InitDragDropFile()
		//{

		//	_scintilla.AllowDrop = true;
		//	_scintilla.DragEnter += delegate (object sender, DragEventArgs e) {
		//		if (e.Data.GetDataPresent(DataFormats.FileDrop))
		//			e.Effect = DragDropEffects.Copy;
		//		else
		//			e.Effect = DragDropEffects.None;
		//	};
		//	_scintilla.DragDrop += delegate (object sender, DragEventArgs e) {

		//		// get file drop
		//		if (e.Data.GetDataPresent(DataFormats.FileDrop))
		//		{

		//			Array a = (Array)e.Data.GetData(DataFormats.FileDrop);
		//			if (a != null)
		//			{

		//				string path = a.GetValue(0).ToString();

		//				LoadDataFromFile(path);

		//			}
		//		}
		//	};

		//}

		//private void LoadDataFromFile(string path)
		//{
		//	if (File.Exists(path))
		//	{
		//		FileName.Text = Path.GetFileName(path);
		//		_scintilla.Text = File.ReadAllText(path);
		//	}
		//}

		#endregion

		#region Main Menu Commands

		//private void openToolStripMenuItem_Click(object sender, EventArgs e)
		//{
		//	if (openFileDialog.ShowDialog() == DialogResult.OK)
		//	{
		//		LoadDataFromFile(openFileDialog.FileName);
		//	}
		//}

		//private void findToolStripMenuItem_Click(object sender, EventArgs e)
		//{
		//	OpenSearch();
		//}

		private void findDialogToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFindDialog();
		}

		private void findAndReplaceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenReplaceDialog();
		}

		private void cutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_scintilla.Cut();
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_scintilla.Copy();
		}

		private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_scintilla.Paste();
		}

		private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_scintilla.SelectAll();
		}

		private void selectLineToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Line line = _scintilla.Lines[_scintilla.CurrentLine];
			_scintilla.SetSelection(line.Position + line.Length, line.Position);
		}

		private void clearSelectionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_scintilla.SetEmptySelection(0);
		}

		//private void indentSelectionToolStripMenuItem_Click(object sender, EventArgs e)
		//{
		//	Indent();
		//}

		//private void outdentSelectionToolStripMenuItem_Click(object sender, EventArgs e)
		//{
		//	Outdent();
		//}

		private void uppercaseSelectionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Uppercase();
		}

		private void lowercaseSelectionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Lowercase();
		}

		//private void wordWrapToolStripMenuItem1_Click(object sender, EventArgs e)
		//{

		//	// toggle word wrap
		//	wordWrapItem.Checked = !wordWrapItem.Checked;
		//	_scintilla.WrapMode = wordWrapItem.Checked ? WrapMode.Word : WrapMode.None;
		//}

		//private void indentGuidesToolStripMenuItem_Click(object sender, EventArgs e)
		//{

		//	// toggle indent guides
		//	indentGuidesItem.Checked = !indentGuidesItem.Checked;
		//	_scintilla.IndentationGuides = indentGuidesItem.Checked ? IndentView.LookBoth : IndentView.None;
		//}

		//private void hiddenCharactersToolStripMenuItem_Click(object sender, EventArgs e)
		//{

		//	// toggle view whitespace
		//	hiddenCharactersItem.Checked = !hiddenCharactersItem.Checked;
		//	_scintilla.ViewWhitespace = hiddenCharactersItem.Checked ? WhitespaceMode.VisibleAlways : WhitespaceMode.Invisible;
		//}

		private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ZoomIn();
		}

		private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ZoomOut();
		}

		private void zoom100ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ZoomDefault();
		}

		private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_scintilla.FoldAll(FoldAction.Contract);
		}

		private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_scintilla.FoldAll(FoldAction.Expand);
		}


		#endregion

		#region Uppercase / Lowercase

		private void Lowercase()
		{

			// save the selection
			int start = _scintilla.SelectionStart;
			int end = _scintilla.SelectionEnd;

			// modify the selected text
			_scintilla.ReplaceSelection(_scintilla.GetTextRange(start, end - start).ToLower());

			// preserve the original selection
			_scintilla.SetSelection(start, end);
		}

		private void Uppercase()
		{

			// save the selection
			int start = _scintilla.SelectionStart;
			int end = _scintilla.SelectionEnd;

			// modify the selected text
			_scintilla.ReplaceSelection(_scintilla.GetTextRange(start, end - start).ToUpper());

			// preserve the original selection
			_scintilla.SetSelection(start, end);
		}

		#endregion

		#region Indent / Outdent

		//private void Indent()
		//{
		//	// we use this hack to send "Shift+Tab" to scintilla, since there is no known API to indent,
		//	// although the indentation function exists. Pressing TAB with the editor focused confirms this.
		//	GenerateKeystrokes("{TAB}");
		//}

		//private void Outdent()
		//{
		//	// we use this hack to send "Shift+Tab" to scintilla, since there is no known API to outdent,
		//	// although the indentation function exists. Pressing Shift+Tab with the editor focused confirms this.
		//	GenerateKeystrokes("+{TAB}");
		//}

		//private void GenerateKeystrokes(string keys)
		//{
		//	HotKeyManager.Enable = false;
		//	_scintilla.Focus();
		//	SendKeys.Send(keys);
		//	HotKeyManager.Enable = true;
		//}

		#endregion

		#region Zoom

		private void ZoomIn()
		{
			_scintilla.ZoomIn();
		}

		private void ZoomOut()
		{
			_scintilla.ZoomOut();
		}

		private void ZoomDefault()
		{
			_scintilla.Zoom = 0;
		}


		#endregion

		#region Quick Search Bar

		//bool SearchIsOpen = false;

		//private void OpenSearch()
		//{

		//	SearchManager.SearchBox = TxtSearch;
		//	SearchManager._scintilla = _scintilla;

		//	if (!SearchIsOpen)
		//	{
		//		SearchIsOpen = true;
		//		InvokeIfNeeded(delegate () {
		//			PanelSearch.Visible = true;
		//			TxtSearch.Text = SearchManager.LastSearch;
		//			TxtSearch.Focus();
		//			TxtSearch.SelectAll();
		//		});
		//	}
		//	else
		//	{
		//		InvokeIfNeeded(delegate () {
		//			TxtSearch.Focus();
		//			TxtSearch.SelectAll();
		//		});
		//	}
		//}
		//private void CloseSearch()
		//{
		//	if (SearchIsOpen)
		//	{
		//		SearchIsOpen = false;
		//		InvokeIfNeeded(delegate () {
		//			PanelSearch.Visible = false;
		//			//CurBrowser.GetBrowser().StopFinding(true);
		//		});
		//	}
		//}

		//private void BtnClearSearch_Click(object sender, EventArgs e)
		//{
		//	CloseSearch();
		//}

		//private void BtnPrevSearch_Click(object sender, EventArgs e)
		//{
		//	SearchManager.Find(false, false);
		//}
		//private void BtnNextSearch_Click(object sender, EventArgs e)
		//{
		//	SearchManager.Find(true, false);
		//}
		//private void TxtSearch_TextChanged(object sender, EventArgs e)
		//{
		//	SearchManager.Find(true, true);
		//}

		//private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
		//{
		//	if (HotKeyManager.IsHotkey(e, Keys.Enter))
		//	{
		//		SearchManager.Find(true, false);
		//	}
		//	if (HotKeyManager.IsHotkey(e, Keys.Enter, true) || HotKeyManager.IsHotkey(e, Keys.Enter, false, true))
		//	{
		//		SearchManager.Find(false, false);
		//	}
		//}

		#endregion

		#region Find & Replace Dialog

		private void OpenFindDialog()
		{

		}
		private void OpenReplaceDialog()
		{


		}

		#endregion

		#region Utils

		public static Color IntToColor(int rgb)
		{
			return Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
		}

		public void InvokeIfNeeded(Action action)
		{
			if (_frm.InvokeRequired)
			{
				_frm.BeginInvoke(action);
			}
			else
			{
				action.Invoke();
			}
		}

		#endregion




	}
}

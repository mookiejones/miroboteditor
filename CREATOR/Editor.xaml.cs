using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.AvalonEdit.Rendering;

namespace CREATOR
{
    /// <summary>
    /// Interaction logic for Editor.xaml
    /// </summary>
    public partial class Editor 
    {
        public static readonly RoutedCommand Comment = new RoutedCommand();
       
        

        
        #region Stuff Comment/Uncomment
        private void CanComment(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }


        private void ExecuteComment(object sender, ExecutedRoutedEventArgs e)
        {
    	    TextDocument document = this.Document;
			DocumentLine start = document.GetLineByOffset(SelectionStart);
			DocumentLine end = document.GetLineByOffset(SelectionStart + SelectionLength);
			using (document.RunUpdate()) {
		    for (DocumentLine line = start; line != end; line = line.NextLine) {
        document.Insert(line.Offset, "// ");
    }

			}}
    	#endregion
    	
    private string imagepath = @"C:\Programming\Images\programicons\vxstruct_icon.png";
      public Editor()
      {
      	this.InitializeComponent();
      	// Add Margin
     

		TextArea.TextEntering += Text_Entering;
      	
      	#region KeyBindings
      	
      	KeyGesture OpenKeyGesture = new KeyGesture( Key.B,    ModifierKeys.Control);

		KeyBinding OpenCmdKeybinding = new KeyBinding( Comment, OpenKeyGesture);
		#endregion

		
		this.InputBindings.Add(OpenCmdKeybinding);
      	
      	//comment binding
         TextArea.CommandBindings.Add(new CommandBinding(Comment, ExecuteComment, CanComment));

      	
      	BitmapImage bitmap = LoadBitmap(imagepath);
      	 var image = new Image {Source = bitmap, Width = bitmap.PixelWidth, Height = bitmap.PixelHeight};
      
     // 	this.TextArea.LeftMargins.Add(image);
      	
      
//      	this.TextArea.LeftMargins.Add(0,image);
		this.Document.TextChanged+= new EventHandler(Editor_TextChanged);
//		this.TextArea.TextView.ElementGenerators.Add( new ImageElementGenerator("c:\\programming\\images"));

		
		this.DataContext=this;
      }

      
      private void AddBookMark(int lineNumber,string type)
      {
      	  	BitmapImage bitmap = LoadBitmap(imagepath);
      	 //	var image = new Image {Source = bitmap, Width = bitmap.PixelWidth, Height = bitmap.PixelHeight};
      		
  
      }
      
    
      
      
      private void Text_Entering(object sender, EventArgs e)
      {
      	
      }
      void FindMatches()
      {
     	int lastFound = 0;
		int nIndex = 0;
      	while ((nIndex = Text.IndexOf("Charles",lastFound))>-1)
      	{
      		var d = Document.GetLineByOffset(nIndex);      			
      			AddBookMark(d.LineNumber,"Charles");
      			lastFound = nIndex+1;
      	}
      	UpdateBookMarks();
        }
      
      
      void UpdateBookMarks()
      {
      	
      }
      
      
      void Editor_TextChanged(object sender, EventArgs e)
      {      
      	FindMatches();
      }
 
  private BitmapImage LoadBitmap(string fileName)
            {
                // TODO: add some kind of cache to avoid reloading the image whenever the
                // VisualLine is reconstructed
                try
                {
                   
                    if (File.Exists(fileName))
                    {
                        BitmapImage bitmap = new BitmapImage(new Uri(fileName));
                        bitmap.Freeze();
                        return bitmap;
                    }
                }
                catch (ArgumentException)
                {
                    // invalid filename syntax
                }
                catch (IOException)
                {
                    // other IO error
                }
                return null;
            }
      


 		 protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            // accept clicks even when clicking on the background
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }
     
        private class Render:IBackgroundRenderer
        {
            private KnownLayer _layer;
            private TextView _view;
            private DrawingContext _context = null;
            public Render(TextView view, DrawingContext context, KnownLayer layer)
            {
                _layer = layer;
                _view = view;
                _context = context;
            }
            #region IBackgroundRenderer Members

            public void Draw(TextView textView, DrawingContext drawingContext)
            {

             
            }
           

            public KnownLayer Layer
            {
                get { return _layer; }
            }

            #endregion
        }

     
  

        protected override void OnRender(DrawingContext drawingContext)
        {
        //    var r = new Render(this.TextArea.TextView,drawingContext,KnownLayer.Background);
  //          this.TextArea.TextView.BackgroundRenderers.Add(r);
        }

        private void InsertSnippet(object sender, KeyEventArgs e)
        {
        	return; 
        	
            var loopCounter = new SnippetReplaceableTextElement {Text = "i"};
            var snippet = new Snippet
                              {
                                  Elements =
                                      {
                                          new SnippetTextElement {Text = "for "},
                                          new SnippetReplaceableTextElement {Text = "item"},
                                          new SnippetTextElement {Text = " in range("},
                                          new SnippetReplaceableTextElement {Text = "from"},
                                          new SnippetTextElement {Text = ", "},
                                          new SnippetReplaceableTextElement {Text = "to"},
                                          new SnippetTextElement {Text = ", "},
                                          new SnippetReplaceableTextElement {Text = "step"},
                                          new SnippetTextElement {Text = "):backN\t"},
                                          new SnippetSelectionElement()
                                      }
                              };
            snippet.Insert(this.TextArea);


        }


    }
}

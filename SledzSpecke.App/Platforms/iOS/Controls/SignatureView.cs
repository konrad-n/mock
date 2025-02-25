using System;
using System.IO;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using UIKit;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace SledzSpecke.App.Platforms.iOS.Controls
{
    public class SignatureView : UIView
    {
        private UIBezierPath _currentPath;
        private UIColor _strokeColor;
        private nfloat _lineWidth;
        private NSMutableArray _paths;
        private bool _isApplePencilMode;
        
        // Właściwości dostępne dla Xamarin.Forms
        public UIColor StrokeColor
        {
            get => _strokeColor;
            set { _strokeColor = value; SetNeedsDisplay(); }
        }
        
        public nfloat LineWidth
        {
            get => _lineWidth;
            set { _lineWidth = value; SetNeedsDisplay(); }
        }
        
        public bool IsApplePencilMode
        {
            get => _isApplePencilMode;
            set { _isApplePencilMode = value; }
        }
        
        public event EventHandler SignatureCompleted;
        
        public SignatureView()
        {
            Initialize();
        }
        
        public SignatureView(CGRect frame) : base(frame)
        {
            Initialize();
        }
        
        private void Initialize()
        {
            _paths = new NSMutableArray();
            _strokeColor = UIColor.Black;
            _lineWidth = 3.0f;
            _isApplePencilMode = true;
            
            MultipleTouchEnabled = false;
            BackgroundColor = UIColor.Clear;
            
            // Skonfiguruj interakcje z Apple Pencil
            if (UIDevice.CurrentDevice.CheckSystemVersion(12, 1))
            {
                var interaction = new UIPencilInteraction();
                interaction.Delegate = new PencilInteractionDelegate(this);
                AddInteraction(interaction);
            }
        }
        
        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            
            // Sprawdź, czy to dotyk Apple Pencil (jeśli tryb Apple Pencil jest włączony)
            if (_isApplePencilMode && UIDevice.CurrentDevice.CheckSystemVersion(9, 1))
            {
                var touch = touches.AnyObject as UITouch;
                if (touch.Type != UITouchType.Stylus)
                {
                    return;
                }
            }
            
            var touch = touches.AnyObject as UITouch;
            var point = touch.LocationInView(this);
            
            _currentPath = UIBezierPath.Create();
            _currentPath.LineWidth = _lineWidth;
            _currentPath.LineCapStyle = CGLineCap.Round;
            _currentPath.LineJoinStyle = CGLineJoin.Round;
            _currentPath.MoveTo(point);
            
            _paths.Add(_currentPath);
            
            SetNeedsDisplay();
        }
        
        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);
            
            if (_currentPath == null)
                return;
                
            var touch = touches.AnyObject as UITouch;
            var point = touch.LocationInView(this);
            
            _currentPath.AddLineTo(point);
            
            SetNeedsDisplay();
        }
        
        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);
            
            _currentPath = null;
            SignatureCompleted?.Invoke(this, EventArgs.Empty);
        }
        
        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);
            
            _currentPath = null;
        }
        
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            
            using (CGContext context = UIGraphics.GetCurrentContext())
            {
                context.SetStrokeColor(_strokeColor.CGColor);
                
                for (nuint i = 0; i < _paths.Count; i++)
                {
                    UIBezierPath path = _paths.GetItem<UIBezierPath>(i);
                    path.Stroke();
                }
            }
        }
        
        public void Clear()
        {
            _paths.RemoveAllObjects();
            SetNeedsDisplay();
        }
        
        public async Task<UIImage> GetSignatureImageAsync()
        {
            UIGraphics.BeginImageContextWithOptions(Bounds.Size, false, 0);
            Layer.RenderInContext(UIGraphics.GetCurrentContext());
            UIImage image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            
            return image;
        }
        
        public async Task<byte[]> GetSignatureAsPngAsync()
        {
            var image = await GetSignatureImageAsync();
            using (NSData data = image.AsPNG())
            {
                byte[] bytes = new byte[data.Length];
                System.Runtime.InteropServices.Marshal.Copy(data.Bytes, bytes, 0, (int)data.Length);
                return bytes;
            }
        }
        
        public async Task<string> SaveSignatureAsync(string filename)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var filePath = Path.Combine(documentsPath, filename);
            
            var imageData = await GetSignatureAsPngAsync();
            File.WriteAllBytes(filePath, imageData);
            
            return filePath;
        }
        
        // Klasa delegata do obsługi interakcji z Apple Pencil
        private class PencilInteractionDelegate : UIPencilInteractionDelegate
        {
            private readonly SignatureView _signatureView;
            
            public PencilInteractionDelegate(SignatureView signatureView)
            {
                _signatureView = signatureView;
            }
            
            public override void PencilInteractionDidTap(UIPencilInteraction interaction, UIPress pencilInfo)
            {
                // Implementacja reakcji na podwójne stuknięcie Apple Pencilem
                // Na przykład, zmiana koloru lub grubości linii
                if (_signatureView._strokeColor == UIColor.Black)
                {
                    _signatureView.StrokeColor = UIColor.Blue;
                }
                else
                {
                    _signatureView.StrokeColor = UIColor.Black;
                }
            }
        }
    }
    
    // Renderer do użycia SignatureView w MAUI
    public class SignatureViewRenderer : Microsoft.Maui.Controls.Handlers.ViewHandler<Xamarin.Forms.View, UIView>
    {
        private SignatureView _signatureView;
        
        protected override UIView CreatePlatformView()
        {
            _signatureView = new SignatureView();
            return _signatureView;
        }
        
        protected override void DisconnectHandler(UIView platformView)
        {
            if (_signatureView != null)
            {
                _signatureView.Dispose();
                _signatureView = null;
            }
            
            base.DisconnectHandler(platformView);
        }
    }
}

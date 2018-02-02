﻿using System;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace CodeRadar
{
    public class CaretFisheyeLineTransformSource : ILineTransformSource
    {
        #region private members
        IWpfTextView _textView;

        private CaretFisheyeLineTransformSource(IWpfTextView textView)
        {
            _textView = textView;

            //Sync to changing the caret position. 
            _textView.Caret.PositionChanged += OnCaretChanged;
        }

        /// <summary>
        /// The caret has been moved by the user. Update line transformations to reflect new position
        /// </summary>
        private void OnCaretChanged(object sender, CaretPositionChangedEventArgs args)
        {
            //Did the caret line number change?
            SnapshotPoint oldPosition = args.OldPosition.BufferPosition;
            SnapshotPoint newPosition = args.NewPosition.BufferPosition;

            if (_textView.TextSnapshot.GetLineNumberFromPosition(newPosition) != 
                _textView.TextSnapshot.GetLineNumberFromPosition(oldPosition))
            {
                //Yes. Is the caret on a line that has been formatted by the view?
                ITextViewLine line = _textView.Caret.ContainingTextViewLine;
                
                if (line.VisibilityState != VisibilityState.Unattached)
                {
                    //Yes. Force the view to redraw so that (top of) the caret line has exactly the same position.
                    _textView.DisplayTextLineContainingBufferPosition(line.Start, line.Top, ViewRelativePosition.Top);
                }
            }
        }
        #endregion

        /// <summary>
        /// Static class factory that ensures a single instance of the line transform source/view.
        /// </summary>
        public static CaretFisheyeLineTransformSource Create(IWpfTextView view)
        {
            return view.Properties.GetOrCreateSingletonProperty(delegate { return new CaretFisheyeLineTransformSource(view); });
        }

        #region ILineTransformSource Members

        /// <summary>
        /// Determine the size of a particular line, based on distance from caret
        /// </summary>
        /// <param name="line">The position of the line currently being modified </param>
        /// <param name="yPosition">Unused data type left over from base.GetLineTransform() </param>
        /// <param name="placement">Unused data type left over from base.GetLineTransform() </param>
        /// <returns>LineTransform object containing the desired scale value for the line</returns>
        public LineTransform GetLineTransform(ITextViewLine line, double yPosition, ViewRelativePosition placement)
        {
            var snapshot = _textView.TextSnapshot;
            int caretLineNumber = snapshot.GetLineNumberFromPosition(_textView.Caret.Position.BufferPosition);
            int lineNumber = snapshot.GetLineNumberFromPosition(line.Start);
            int delta = Math.Abs(caretLineNumber - lineNumber);

            if (delta <= 5)
            {
                var text = snapshot.GetText(line.Start, line.Length);
                var emptyLine = string.IsNullOrWhiteSpace(text);
                if (emptyLine)
                    return new LineTransform(0, 0, 0.6);
                else
                    return new LineTransform(0, 0, 1.3);
            }

            return new LineTransform(0, 0, 0.6);
        }
        #endregion
    }
}

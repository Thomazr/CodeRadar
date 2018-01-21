﻿using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Utilities;

namespace CodeRadar
{
    /// <summary>
    /// This class implements a connector that produces the CaretFisheye LineTransformSourceProvider.
    /// </summary>
    [Export(typeof(ILineTransformSourceProvider))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    internal sealed class CaretFisheyeLineTransformSourceProvider : ILineTransformSourceProvider
    {
        public ILineTransformSource Create(IWpfTextView textView)
        {
            return CaretFisheyeLineTransformSource.Create(textView);
        }
    }
}


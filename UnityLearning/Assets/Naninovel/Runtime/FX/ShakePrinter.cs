﻿// Copyright 2017-2019 Elringus (Artyom Sovetnikov). All Rights Reserved.

using UnityCommon;
using UnityEngine;

namespace Naninovel.FX
{
    /// <summary>
    /// Shakes a <see cref="ITextPrinterActor"/> with provided ID or an active one.
    /// </summary>
    public class ShakePrinter : ShakeTransform
    {
        protected override Transform GetShakedTransform ()
        {
            var mngr = Engine.GetService<TextPrinterManager>();
            var id = string.IsNullOrEmpty(ObjectName) ? mngr.DefaultPrinterId : ObjectName;
            var uiRoot = GameObject.Find(id);
            if (!ObjectUtils.IsValid(uiRoot)) return null;
            // Changing transform of the UI root won't work; use the content instead.
            return uiRoot.transform.FindRecursive("Content");
        }
    }
}

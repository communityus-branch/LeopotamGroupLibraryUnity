// ----------------------------------------------------------------------------
// The MIT License
// LeopotamGroupLibrary https://github.com/Leopotam/LeopotamGroupLibraryUnity
// Copyright (c) 2012-2017 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using LeopotamGroup.Math;
using LeopotamGroup.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LeopotamGroup.SystemUi.Markup.Generators {
    static class ImageNode {
        static readonly int HashedPath = "path".GetStableHashCode ();

        static readonly int HashedRaw = "raw".GetStableHashCode ();

        static readonly int HashedNativeSize = "nativeSize".GetStableHashCode ();

        /// <summary>
        /// Create "image" node.
        /// </summary>
        /// <param name="go">Gameobject holder.</param>
        /// <param name="node">Xml node.</param>
        /// <param name="container">Markup container.</param>
        public static void Create (GameObject go, XmlNode node, MarkupContainer container) {
#if UNITY_EDITOR
            go.name = "image";
#endif
            Image img = null;
            RawImage tex = null;
            string attrValue;
            var useImg = true;
            var ignoreSize = false;

            attrValue = node.GetAttribute (HashedRaw);
            if (string.CompareOrdinal (attrValue, "true") == 0) {
                useImg = false;
                tex = go.AddComponent<RawImage> ();
            } else {
                img = go.AddComponent<Image> ();
            }

            attrValue = node.GetAttribute (HashedPath);
            if (!string.IsNullOrEmpty (attrValue)) {
                if (useImg) {
                    // Image.
                    var parts = MarkupUtils.SplitAttrValue (attrValue);
                    if (parts.Length == 2) {
                        var atlas = container.GetAtlas (parts[0]);
                        if ((object) atlas != null) {
                            img.sprite = atlas.Get (parts[1]);
                        }
                    }
                } else {
                    // RawImage.
                    tex.texture = Resources.Load<Texture2D> (attrValue);
                }
            }

            if (useImg) {
                attrValue = node.GetAttribute (HashedNativeSize);
                ignoreSize = (object) img.sprite != null && string.CompareOrdinal (attrValue, "true") == 0;
            }

            if (ignoreSize) {
                img.SetNativeSize ();
            } else {
                MarkupUtils.SetSize (go, node);
            }

            MarkupUtils.SetColor (img, node);
            MarkupUtils.SetRotation (go, node);
            MarkupUtils.SetOffset (go, node);
            MarkupUtils.SetMask (go, node);
            MarkupUtils.SetHidden (go, node);
            var isInteractive = MarkupUtils.ValidateInteractive (go, node);
            if (useImg) {
                img.raycastTarget = isInteractive;
            } else {
                tex.raycastTarget = isInteractive;
            }
        }
    }
}
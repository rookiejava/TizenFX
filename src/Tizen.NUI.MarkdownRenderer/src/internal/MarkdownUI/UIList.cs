﻿/*
 * Copyright(c) 2025 Samsung Electronics Co., Ltd.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

using System;
using System.ComponentModel;

using Tizen.NUI.BaseComponents;

namespace Tizen.NUI.MarkdownRenderer
{
    /// <summary>
    /// Renders a list item as a horizontal layout with a bullet and styled text.
    /// Used for unordered or ordered markdown list items.
    /// </summary>
    internal class UIListItemParagraph : View
    {
        private readonly ListStyle list;
        private readonly ParagraphStyle paragraph;

        private UIParagraph uiParagraph;

        public string ContentHash { get; set; } = string.Empty;

        public string Text
        {
            get => uiParagraph.Text;
            set => uiParagraph.Text = value;
        }

        public UIListItemParagraph(string text, ListStyle listStyle, ParagraphStyle paragraphStyle, string hash, bool asyncRendering) : base()
        {
            ContentHash = hash;
            list = listStyle;
            paragraph = paragraphStyle;
            SetupLayout();

            Add(CreateBullet());
            uiParagraph = new UIParagraph(text, paragraph, string.Empty, asyncRendering);
            Add(uiParagraph);
        }

        public UIListItemParagraph(string text, int number, ListStyle listStyle, ParagraphStyle paragraphStyle, string hash, bool asyncRendering) : base()
        {
            ContentHash = hash;
            list = listStyle;
            paragraph = paragraphStyle;
            SetupLayout();

            Add(CreateNumber(number, asyncRendering));
            uiParagraph = new UIParagraph(text, paragraph, string.Empty, asyncRendering);
            Add(uiParagraph);
        }

        private void SetupLayout()
        {
            Layout = new MarkdownLinearLayout() {};
            WidthSpecification = LayoutParamPolicies.MatchParent;
            Margin = new Extents(0, 0, (ushort)list.ItemMarginTop, (ushort)list.ItemMarginBottom);
        }

        private View CreateBullet()
        {
            int bulletSize = UIList.GetBulletSize(paragraph);
            ushort bulletMargin = UIList.GetBulletMargin(bulletSize, paragraph);
            var bullet = new View()
            {
                WidthSpecification = bulletSize,
                HeightSpecification = bulletSize,
                BackgroundColor = new Color(paragraph.FontColor),
                Margin = new Extents(bulletMargin),
            };
            return bullet;
        }

        private UIParagraph CreateNumber(int number, bool asyncRendering)
        {
            int numberSize = UIList.GetNumberSize(paragraph);
            ushort numberPadding = UIList.GetNumberPadding(paragraph);
            string text = MarkdownRenderer.IsRTL ? $".{number}" : $"{number}.";
            var numberParagraph = new UIParagraph(text, paragraph)
            {
                WidthSpecification = number < 10 ? numberSize : numberSize + numberPadding,
                HeightSpecification = numberSize,
                HorizontalAlignment = HorizontalAlignment.Center,
                Padding = new Extents(0, numberPadding, 0, 0),
                RenderMode = asyncRendering ? TextRenderMode.AsyncAuto : TextRenderMode.Sync,
            };
            return numberParagraph;
        }
    }

    /// <summary>
    /// Represents a single list item container with vertical layout and indentation.
    /// Used to group one or more list item paragraphs or nested lists.
    /// </summary>
    internal class UIListItem : View
    {
        private readonly CommonStyle common;

        public UIListItem(CommonStyle commonStyle) : base()
        {
            common = commonStyle;
            SetupLayout();
        }

        private void SetupLayout()
        {
            Layout = new MarkdownLinearLayout()
            {
                LinearOrientation = LinearLayout.Orientation.Vertical,
            };
            WidthSpecification = LayoutParamPolicies.MatchParent;
            Margin = new Extents((ushort)common.Indent, 0, 0, 0);
        }
    }

    /// <summary>
    /// Represents a markdown list (ordered or unordered) with vertical layout and outer margin.
    /// Acts as a container for list items.
    /// </summary>
    internal class UIList : View
    {
        public static int GetBulletSize(ParagraphStyle paragraph)
        {
            return Math.Max(2, (int)Math.Round(paragraph.FontSize / 4));
        }

        public static ushort GetBulletMargin(int bulletSize, ParagraphStyle paragraph)
        {
            return (ushort)Math.Round((paragraph.LineHeight - bulletSize) / 2);
        }

        public static int GetNumberSize(ParagraphStyle paragraph)
        {
            return (int)paragraph.LineHeight;
        }

        public static ushort GetNumberPadding(ParagraphStyle paragraph)
        {
            return (ushort)Math.Round(paragraph.LineHeight / 4);
        }

        public UIList() : base()
        {
            SetupLayout();
        }

        private void SetupLayout()
        {
            Layout = new MarkdownLinearLayout()
            {
                LinearOrientation = LinearLayout.Orientation.Vertical,
            };
            WidthSpecification = LayoutParamPolicies.MatchParent;
        }
    }
}

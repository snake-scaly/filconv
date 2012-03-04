using System;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;

namespace FilConvWpf
{
    /// <summary>
    /// Simplifies working with a portion of a toolbar.
    /// </summary>
    class ToolbarFragment
    {
        private ToolBar _toolbar;
        private object _leftAnchor;
        private bool _leftSeparator;
        private object _rightAnchor;
        private bool _rightSeparator;

        private ToolbarFragment(ToolBar toolbar, object leftAnchor, bool leftSeparator, object rightAnchor, bool rightSeparator)
        {
            _toolbar = toolbar;
            _leftAnchor = leftAnchor;
            _leftSeparator = leftSeparator;
            _rightAnchor = rightAnchor;
            _rightSeparator = rightSeparator;
        }

        /// <summary>
        /// Create a toolbar fragment between the given toolbar elements.
        /// </summary>
        /// <param name="leftAnchor">left boundary of the fragment, or <code>null</code></param>
        /// <param name="rightAnchor">right boundary of the fragment, or <code>null</code></param>
        public ToolbarFragment(ToolBar toolbar, object leftAnchor, object rightAnchor)
            : this(toolbar, leftAnchor, leftAnchor != null, rightAnchor, rightAnchor != null)
        {
        }

        /// <summary>
        /// Get a fragment of a fragment
        /// </summary>
        /// <param name="leftAnchor"></param>
        /// <param name="rightAnchor"></param>
        /// <returns></returns>
        public ToolbarFragment GetFragment(object leftAnchor, object rightAnchor)
        {
            return new ToolbarFragment(
                _toolbar,
                leftAnchor ?? _leftAnchor,
                leftAnchor != null,
                rightAnchor ?? _rightAnchor,
                rightAnchor != null);
        }

        public void Clear()
        {
            int left = GetFragmentStart();
            int count = GetFragmentEnd() - left;
            for (int i = 0; i < count; ++i)
            {
                _toolbar.Items.RemoveAt(left);
            }
        }

        public void Add(object item)
        {
            int left = GetFragmentStart();
            if (left == GetFragmentEnd())
            {
                if (_rightSeparator)
                {
                    _toolbar.Items.Insert(left, new Separator());
                }
                if (_leftSeparator)
                {
                    _toolbar.Items.Insert(left, new Separator());
                }
            }
            _toolbar.Items.Insert(GetItemsEnd(), item);
        }

        private int GetFragmentStart()
        {
            if (_leftAnchor == null)
            {
                return 0;
            }

            int left = _toolbar.Items.IndexOf(_leftAnchor);
            if (left == -1)
            {
                throw new InvalidProgramException("Left anchor does not exist in the toolbar");
            }
            return left + 1;
        }

        private int GetFragmentEnd()
        {
            int right = _rightAnchor != null ? _toolbar.Items.IndexOf(_rightAnchor) : _toolbar.Items.Count;
            if (right == -1)
            {
                throw new InvalidProgramException("Right anchor does not exist in the toolbar");
            }
            return right;
        }

        private int GetItemsStart()
        {
            int itemsStart = GetFragmentStart();
            if (_leftSeparator && itemsStart < GetFragmentEnd())
            {
                itemsStart++;
            }
            return itemsStart;
        }

        private int GetItemsEnd()
        {
            int itemsEnd = GetFragmentEnd();
            if (_rightSeparator && itemsEnd > GetItemsStart())
            {
                itemsEnd--;
            }
            return itemsEnd;
        }
    }
}

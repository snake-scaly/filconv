using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FilConvWpf
{
    interface IToolbarClient
    {
        /// <summary>
        /// Grant the toolbar client an exclusive access to a toolbar fragment.
        /// </summary>
        /// <remarks>
        /// Only one fragment may be granted at any given time.
        /// </remarks>
        void GrantToolbarFragment(ToolbarFragment fragment);

        /// <summary>
        /// Revoke toolbar access.
        /// </summary>
        /// <remarks>
        /// The client must not access the fragment after this method returns.
        /// </remarks>
        void RevokeToolbarFragment();
    }
}

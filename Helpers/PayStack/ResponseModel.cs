using System;
using System.Collections.Generic;
using System.Text;

namespace Helpers
{
    public class ResponseModel
    {
        #region Properties

        /// <summary>Gets or sets a value indicating whether this <see cref="ResponseModel"/> is success.</summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.</value>
        public bool Status { get; set; }

        /// <summary>Gets or sets the data.</summary>
        /// <value>The data.</value>
        public object Data { get; set; }

        /// <summary>Gets or sets the message.</summary>
        /// <value>The message.</value>
        public string Message { get; set; }

        #endregion

        #region Constructor

        /// <summary>Initializes a new instance of the <see cref="ResponseModel"/> class.</summary>
        public ResponseModel()
        {
            this.Data = null;
        }

        #endregion
    }
}

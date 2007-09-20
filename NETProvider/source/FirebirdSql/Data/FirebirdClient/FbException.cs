/*
 *  Firebird ADO.NET Data provider for .NET and Mono 
 * 
 *     The contents of this file are subject to the Initial 
 *     Developer's Public License Version 1.0 (the "License"); 
 *     you may not use this file except in compliance with the 
 *     License. You may obtain a copy of the License at 
 *     http://www.firebirdsql.org/index.php?op=doc&id=idpl
 *
 *     Software distributed under the License is distributed on 
 *     an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either 
 *     express or implied.  See the License for the specific 
 *     language governing rights and limitations under the License.
 * 
 *  Copyright (c) 2002, 2007 Carlos Guzman Alvarez
 *  All Rights Reserved.
 */

using System;
using System.ComponentModel;
using System.Data.Common;
#if (!NETCF)
	using System.Runtime.Serialization;
#endif
using System.Security.Permissions;
using FirebirdSql.Data.Common;

namespace FirebirdSql.Data.FirebirdClient
{
	/// <include file='Doc/en_EN/FbException.xml' path='doc/class[@name="FbException"]/overview/*'/>
#if (!NETCF)
	[Serializable] 
	public sealed class FbException : DbException
#else
	public sealed class FbException : SystemException
#endif
	{
		#region � Fields �
		
		private FbErrorCollection errors;
		
		#endregion

		#region � Properties �

		/// <include file='Doc/en_EN/FbException.xml' path='doc/class[@name="FbException"]/property[@name="Errors"]/*'/>
#if (!NETCF)
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#endif
		public FbErrorCollection Errors
		{
			get 
            {
                if (this.errors == null)
                {
                    this.errors = new FbErrorCollection();
                }

                return this.errors; 
            }
		}

#if (!NETCF)

        public override int ErrorCode
        {
            get
            {
                if ((this.InnerException != null) && (this.InnerException is IscException))
                {
                    return ((IscException)this.InnerException).ErrorCode;
                }

                return base.ErrorCode;
            }
        }

#endif

		#endregion

		#region � Constructors �

		internal FbException() 
			: base()
		{
		}

		internal FbException(string message) 
			: base(message)
		{
		}

		internal FbException(string message, Exception innerException) 
			: base(message, innerException)
		{
            if (innerException is IscException)
            {
                this.GetIscExceptionErrors((IscException)innerException);
            }
		}

#if (!NETCF)

		internal FbException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.errors	 = (FbErrorCollection)info.GetValue("errors", typeof(FbErrorCollection));
		}

#endif

		#endregion

		#region � Methods �

#if (!NETCF)

		/// <include file='Doc/en_EN/FbException.xml' path='doc/class[@name="FbException"]/method[@name="GetObjectData(SerializationInfo, StreamingContext)"]/*'/>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter=true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("errors", this.errors);

			base.GetObjectData(info, context);
		}

#endif

		#endregion

		#region � Internal Methods �

		internal void GetIscExceptionErrors(IscException innerException)
		{
            foreach (IscError error in innerException.Errors)
			{
				this.Errors.Add(error.Message, error.ErrorCode);
			}
		}

		#endregion
	}
}

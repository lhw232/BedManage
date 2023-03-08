using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BedAppManage.Models
{
	/// <summary>
	/// 轮播图实体类 
	/// </summary>
	[Serializable]
	public class LunboInfo
	{
		int _no;
		string _img;
		int _orderNo;

		///<summary>
		///
		///</summary>
		public int no
		{
			get { return _no; }
			set { _no = value; }
		}

		///<summary>
		///
		///</summary>
		public string img
		{
			get
			{
				if (_img == null)
					return String.Empty;
				else
					return _img;
			}
			set { _img = value; }
		}

		///<summary>
		///
		///</summary>
		public int orderNo
		{
			get { return _orderNo; }
			set { _orderNo = value; }
		}

		/// <summary>
		/// 深度克隆自身；
		/// </summary>
		/// <returns></returns>
		public LunboInfo Clone()
		{
			LunboInfo entity = new LunboInfo();

			entity.no = this.no;
			entity.img = this.img;
			entity.orderNo = this.orderNo;

			return entity;
		}

	} //class end.
}
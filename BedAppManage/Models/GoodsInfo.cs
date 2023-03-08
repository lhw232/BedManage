using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BedAppManage.Models
{
	/// <summary>
	/// 商品表实体类 
	/// </summary>
	[Serializable]
	public class GoodsInfo
	{
		int _no;
		int _typeNo;
		string _name;
		string _price;
		string _falsePrice;
		string _img1;
		string _img2;
		string _img3;

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
		public int typeNo
		{
			get { return _typeNo; }
			set { _typeNo = value; }
		}

		///<summary>
		///
		///</summary>
		public string name
		{
			get
			{
				if (_name == null)
					return String.Empty;
				else
					return _name;
			}
			set { _name = value; }
		}

		///<summary>
		///
		///</summary>
		public string price
		{
			get
			{
				if (_price == null)
					return String.Empty;
				else
					return _price;
			}
			set { _price = value; }
		}

		///<summary>
		///
		///</summary>
		public string falsePrice
		{
			get
			{
				if (_falsePrice == null)
					return String.Empty;
				else
					return _falsePrice;
			}
			set { _falsePrice = value; }
		}

		///<summary>
		///
		///</summary>
		public string img1
		{
			get
			{
				if (_img1 == null)
					return String.Empty;
				else
					return _img1;
			}
			set { _img1 = value; }
		}

		///<summary>
		///
		///</summary>
		public string img2
		{
			get
			{
				if (_img2 == null)
					return String.Empty;
				else
					return _img2;
			}
			set { _img2 = value; }
		}

		///<summary>
		///
		///</summary>
		public string img3
		{
			get
			{
				if (_img3 == null)
					return String.Empty;
				else
					return _img3;
			}
			set { _img3 = value; }
		}

		/// <summary>
		/// 深度克隆自身；
		/// </summary>
		/// <returns></returns>
		public GoodsInfo Clone()
		{
			GoodsInfo entity = new GoodsInfo();

			entity.no = this.no;
			entity.typeNo = this.typeNo;
			entity.name = this.name;
			entity.price = this.price;
			entity.falsePrice = this.falsePrice;
			entity.img1 = this.img1;
			entity.img2 = this.img2;
			entity.img3 = this.img3;

			return entity;
		}

	} //class end.
}
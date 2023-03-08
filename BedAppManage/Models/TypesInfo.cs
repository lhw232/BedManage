using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BedAppManage.Models
{
	/// <summary>
	/// 商品分类表实体类 
	/// </summary>
	[Serializable]
	public class TypesInfo
	{
		int _no;
		string _name;
		string _img;

		///<summary>
		///分类编号
		///</summary>
		public int no
		{
			get { return _no; }
			set { _no = value; }
		}

		///<summary>
		///分类名称
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
		///分类图片
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

		/// <summary>
		/// 深度克隆自身；
		/// </summary>
		/// <returns></returns>
		public TypesInfo Clone()
		{
			TypesInfo entity = new TypesInfo();

			entity.no = this.no;
			entity.name = this.name;
			entity.img = this.img;

			return entity;
		}

	} //class end.
}
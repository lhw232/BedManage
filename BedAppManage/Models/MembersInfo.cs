using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BedAppManage.Models
{
	/// <summary>
	/// 用户表实体类 
	/// </summary>
	[Serializable]
	public class MembersInfo
	{
		int _no;
		int? _phone;
		string _nickname;
		string _faceurl;
		string _sex;
		DateTime? _birthday;
		string _state;
		string _PassWord;

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
		public int? phone
		{
			get { return _phone; }
			set { _phone = value; }
		}

		///<summary>
		///
		///</summary>
		public string nickname
		{
			get
			{
				if (_nickname == null)
					return String.Empty;
				else
					return _nickname;
			}
			set { _nickname = value; }
		}

		///<summary>
		///
		///</summary>
		public string faceurl
		{
			get
			{
				if (_faceurl == null)
					return String.Empty;
				else
					return _faceurl;
			}
			set { _faceurl = value; }
		}

		///<summary>
		///
		///</summary>
		public string sex
		{
			get
			{
				if (_sex == null)
					return String.Empty;
				else
					return _sex;
			}
			set { _sex = value; }
		}

		///<summary>
		///
		///</summary>
		public DateTime? birthday
		{
			get { return _birthday; }
			set { _birthday = value; }
		}

		///<summary>
		///
		///</summary>
		public string state
		{
			get
			{
				if (_state == null)
					return String.Empty;
				else
					return _state;
			}
			set { _state = value; }
		}

		public string passWord
		{
			get
			{
				if (_PassWord == null)
					return String.Empty;
				else
					return _PassWord;
			}
			set { _PassWord = value; }
		}

		/// <summary>
		/// 深度克隆自身；
		/// </summary>
		/// <returns></returns>
		public MembersInfo Clone()
		{
			MembersInfo entity = new MembersInfo();

			entity.no = this.no;
			entity.phone = this.phone;
			entity.nickname = this.nickname;
			entity.faceurl = this.faceurl;
			entity.sex = this.sex;
			entity.birthday = this.birthday;
			entity.state = this.state;
			entity.passWord = this.passWord;

			return entity;
		}

	} //class end.
}
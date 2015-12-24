using System.Collections;
using System.Collections.Generic;
// コルーチンを定義
//ここでは情報の型？コンテナのような役割のみもちます.
//

namespace MyLib
{
	public class Coroutine
	{

		public string _method_name;
		public IEnumerator _routine;
		public bool _is_chained;
		public LinkedListNode<Coroutine> _node;

		//①メソッド名と、イテレーターを保存
		public Coroutine(string methodName, IEnumerator routine)
			: this(methodName, routine, false)
		{
		}
		//②メソッド名と、イテレーター、コルーチンがネストしているかを保存
		public Coroutine(string methodName, IEnumerator routine,  bool isChained)
		{
			_method_name = methodName;
			_routine = routine;
			_is_chained = isChained;
		}

	}



}
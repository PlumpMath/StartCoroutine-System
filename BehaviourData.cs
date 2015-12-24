//
// ゲームエンジンが、Monovihaviourを保持する際に
// 使用するクラスでございます。
//
//
using System.Collections.Generic;

namespace MyLib
{
	public class BehaviourData
	{

		public MyLib.MonoBehaviour _behaviour;  
		public bool _is_main_loop;				           //Update()のループが始まった際にtrue
		public LinkedList<LinkedList<MyLib.Coroutine>> _routine_list;  //コルーチンの集合を保存

		public BehaviourData(MyLib.MonoBehaviour behaviour)
		{
			_behaviour = behaviour;
			_is_main_loop = false;
			_routine_list = new LinkedList<LinkedList<MyLib.Coroutine>>();
		}

	}


}
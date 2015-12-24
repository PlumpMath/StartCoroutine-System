using System.Collections;
using System.Collections.Generic; //Dictionary<,> で使用.

//
//ここで、ユーザーが定義したスクリプトに対して  
//Start() と　Update()を呼び出します。（呼び出す責任を持ってる）
//

public class Main : UnityEngine.MonoBehaviour {

	//ユーザー定義のスクリプトをキーにして、その詳細情報を保持するDictionary
	private static Dictionary<MyLib.MonoBehaviour,MyLib.BehaviourData> _behavior_dict = 
		new Dictionary<MyLib.MonoBehaviour,MyLib.BehaviourData>();


	//ユーザー定義のスクリプト(My.Libを継承したクラス)を
	//Dictionary(_behavior_dict)に登録.
	public static void AddMonoBehavior(MyLib.MonoBehaviour behaviour)
	{
		//すでに登録していなければ
		if (!_behavior_dict.ContainsKey (behaviour)) 
		{
			
			_behavior_dict.Add (behaviour,new MyLib.BehaviourData(behaviour));

		}

	}

	//ゲームエンジンにイテレーターを登録致します。
	//WARNING:このメソッドの実行中に StartCoroutine() が呼ばれると再入するので注意。
	public static MyLib.Coroutine AddRoutine
	(MyLib.MonoBehaviour behaviour, string method_name, IEnumerator routine)
	{

		MyLib.BehaviourData b_data;
		//スクリプトと同じ名前のキーから、値を取得する
		if (_behavior_dict.TryGetValue (behaviour, out b_data)) {

			//コルーチンを生成、初期化
			var coroutine = new MyLib.Coroutine (method_name, routine);

			//コルーチンを登録
			var coroutine_list = new LinkedList<MyLib.Coroutine> ();
			coroutine._node = coroutine_list.AddLast(coroutine);
			b_data._routine_list.AddLast(coroutine_list);

			// コルーチンの初回実行を行う。
			ProcessCoroutine(coroutine);

			return coroutine;
		}
		else 
		{
			UnityEngine.Debug.Log ("登録が行われませんでした。Null値を返します");
			return null;

		}

	}



	/// コルーチンの実行。コルーチンが既に終了していたらfalseを返す。
	private static bool ProcessCoroutine(MyLib.Coroutine coroutine)
	{
		// 一回だけ実行
		bool executed = coroutine._routine.MoveNext();

		if (executed)
		{
			object current = coroutine._routine.Current;

			// current は yield return の戻り値である。
			if (current is MyLib.Coroutine)
			{
				MyLib.Coroutine nested = (MyLib.Coroutine)current;

				if (nested._is_chained)
				{
					UnityEngine.Debug.Log("[エラー] 1つのコルーチンで2つ以上のコルーチンを待機させる事はできません。");
				}
				else
				{
					// nestedが登録されているLinkedListからnestedを削除。
					nested._node.List.Remove(nested._node);
					// coroutineのリストに改めてnestedを登録。
					nested._node = coroutine._node.List.AddLast(nested);
					// nestedはコルーチンチェーンに組み込まれたので、フラグを立てる。
					nested._is_chained = true;
				}
			}
		}

		return executed;
	}


	//名前で指定したコルーチンを、実行対象から外す。　※MonoBehavior.StopCorotine()でよばれます
	public static void RemoveRoutine(MyLib.MonoBehaviour behaviour,string method_name)
	{
		MyLib.BehaviourData b_data;
		if (_behavior_dict.TryGetValue (behaviour, out b_data))
		{
			//リストの頭のデータを取得、頭から指定した名前と一致するコルーチンを検索
			LinkedListNode<LinkedList<MyLib.Coroutine>> node = b_data._routine_list.First;
			while(node != null)
			{

				LinkedList<MyLib.Coroutine> list = node.Value;
				RemoveRoutineSub(list,method_name);

				var old_node = node;
				node = node.Next;

				//listの要素が空になったら
				if (list.Count == 0) 
				{
					b_data._routine_list.Remove(old_node);
				}

			}

		}
	}

	private static void RemoveRoutineSub(LinkedList<MyLib.Coroutine> list, string methodName)
	{
		LinkedListNode<MyLib.Coroutine> node = list.First;
		while (node != null)
		{
			var old_node = node;
			node = node.Next;
			if (old_node.Value._method_name == methodName)
			{
				list.Remove(old_node);
			}
		}
	}

	//登録した全てのコルーチンを削除
	public static void RemoveAllRoutines(MyLib.MonoBehaviour behaviour)
	{
		MyLib.BehaviourData b_data;
		if (_behavior_dict.TryGetValue (behaviour, out b_data)) 
		{

			b_data._routine_list.Clear ();

		}


	}

	private void ProcessChainedCoroutine(LinkedList<MyLib.Coroutine> chain)
	{
		// chainの末尾を実行。
		// 実行完了していたら、chainから削除。

		LinkedListNode<MyLib.Coroutine> node = chain.Last;
		if (node != null)
		{
			MyLib.Coroutine coroutine = node.Value;

			if (ProcessCoroutine(coroutine))
			{
				node = node.Next;
			}
			else
			{
				// 終わったコルーチンはリストから除外
				LinkedListNode<MyLib.Coroutine> toRemove = node;
				node = node.Next;
				chain.Remove(toRemove);
			}
		}
	}

	//
	//
	//
	// 実行部
	//
	//
	// Use this for initialization
	void Awake () {
	

		AddMonoBehavior (new Test());


	}
	
	// Update is called once per frame
	void Update () {
	
		//全てのMonoBehviourを実行
		foreach (MyLib.BehaviourData bdata in _behavior_dict.Values)
		{

			if (!bdata._is_main_loop) 
			{

				bdata._behaviour.Start ();
				bdata._is_main_loop = true;

			}
			bdata._behaviour.Update();

		}

		//コルーチンを実行,尚,実行完了後のイテレーターは連結リストから除外
		foreach (MyLib.BehaviourData b_data in _behavior_dict.Values) {

			//登録された、コルーチンを実行
			LinkedListNode<LinkedList<MyLib.Coroutine>> node = b_data._routine_list.First;
			while (node != null) {
			
				LinkedList<MyLib.Coroutine> coroutine_chain = node.Value;
				ProcessChainedCoroutine(coroutine_chain);

				var old_node = node;
				node = node.Next;

				// コルーチンチェーンが空になったら、チェーンの入れ物自体を破棄。
				if (coroutine_chain.Count == 0)
				{
					b_data._routine_list.Remove(old_node);
				}
			}

		}

	}
		


}

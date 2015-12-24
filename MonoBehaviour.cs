using UnityEngine;
using System.Collections;
using System.Reflection;
// 勉強スクリプトなので無駄なコメントが多いのは、ご理解申し上げます。
//
// Unityだけど、ユーザー自身で、
// 定義したスクリプトを実行できるようしてみたいぜ.
// Unityのスタートコルーチンの中身を簡略化ではあるが、書いてみる。
// written by sato takumi

namespace MyLib
{

	public class MonoBehaviour
	{

		//MonoBehaviorのメソッドをオーバーライドすることで、
		//ゲームエンジン側からのコールバックを受け取るようにする。
		public virtual void Start(){}					   //復習コメント:仮想メソッド <virtual>
		public virtual void Update(){}                     //動的な型に基づいて、呼び出されるメソッドを
														   //変更したい場合に使うのですね

		//①：イテレーターを直接受けって行うスタートコルーチン
		//①,②の主な処理はStartCoroutineCommon()に丸投げ
		public MyLib.Coroutine StartCoroutine(IEnumerator routine)
		{
			return StartCoroutineCommon(null, routine);
		}
		//②:関数名をうけとって行うコルーチン.
		public MyLib.Coroutine StartCoroutine(string methodName, object arg = null)
		{
			object[] param = (arg == null) ?
				null :
				new object[] { arg };

			//イテレーターを取得
			IEnumerator routine = (IEnumerator)this.GetType().InvokeMember(
				methodName,
				BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic,
				null, this, param
			);

			return StartCoroutineCommon(methodName, routine);
		}

		//
		private MyLib.Coroutine StartCoroutineCommon(string methodName, IEnumerator routine)
		{
			return Main.AddRoutine(this, methodName, routine);
		}

		public void StopCoroutine(string methodName)
		{
			Main.RemoveRoutine(this, methodName);
		}

		public void StopAllCoroutines()
		{
			Main.RemoveAllRoutines(this);
		}

	}



}


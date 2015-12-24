using MyLib;
using System.Collections;

//
// コルーチン テスト用
//
//

public class Test : MyLib.MonoBehaviour{

	public override void Start()
	{
		//test用、
		//①イテレーターを直接ぶち込むタイプ
		//②メソッド名からぶち込むタイプ
		//StartCoroutine(RoutineTest("A"));
		//StartCoroutine("RoutineTest", "B");

		//①−２ネストしてるタイプのテスト
		StartCoroutine(Routine1());

		UnityEngine.Debug.Log ("Test.csのStartを実行");

	}

	public override void Update()
	{
		//UnityEngine.Debug.Log ("Test.csのUpdateを実行");

	}

	//テスト用コルーチン
	private IEnumerator RoutineTest(string name)
	{
		int count = 0;

		while (true)
		{
			++count;

			if (count == 5)
			{
				StopCoroutine("RoutineTest");
			}
			if (count == 10)
			{
				yield break;
			}

			UnityEngine.Debug.Log(">>> " + name + ", " + count);
			UnityEngine.Debug.Log(UnityEngine.Time.time);

			yield return null;
		}
	}

	IEnumerator Routine1()
	{
		UnityEngine.Debug.Log("Routine1 In");

		yield return StartCoroutine(Routine2());

		UnityEngine.Debug.Log("Routine1 Out");
	}

	IEnumerator Routine2()
	{
		UnityEngine.Debug.Log("    Routine2 In");
		yield return null;
		yield return StartCoroutine(Routine3());
		UnityEngine.Debug.Log("    Routine2 Out");
	}

	IEnumerator Routine3()
	{
		UnityEngine.Debug.Log("        Routine3 In");
		yield return null;
		UnityEngine.Debug.Log("        Routine3 Out");
	}
}

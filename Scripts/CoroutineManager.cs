using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//StartCoroutine과 yield return은 각각 가비지를 만들어낸다. startcoroutine에 경우 stopcoroutine이 필요하기 때문에
//가비지가 생성되는 것이다. stopcoroutine이 필요없는 코루틴매니저를 UniRx에는 따로 코드를 지원해준다.
//잘못봐서 unirx가 지원되는 줄 알았다->일단 클래스는 남겨두고 yield return만쓰기로
public class MicroCoroutine
{
    List<IEnumerator> _coroutines = new List<IEnumerator>();

    public void AddCoroutine(IEnumerator enumerator)
    {
        _coroutines.Add(enumerator);
    }

    public void Run()
    {
        int i = 0;
        while (i < _coroutines.Count)
        {
            if (!_coroutines[i].MoveNext())
            {
                _coroutines.RemoveAt(i);
                continue;
            }
            i++;
        }
    }
}

public class CoroutineManager : MonoBehaviour
{
    #region Singleton

    private static CoroutineManager _instance;
    public static CoroutineManager instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject();
                go.name = "~CoroutineManager";
                DontDestroyOnLoad(go);
                _instance = go.AddComponent<CoroutineManager>();
            }
            return _instance;
        }
    }

    #endregion

    MicroCoroutine updateMicroCoroutine = new MicroCoroutine();
    MicroCoroutine fixedUpdateMicroCoroutine = new MicroCoroutine();
    MicroCoroutine endOfFrameMicroCoroutine = new MicroCoroutine();

    public static void StartUpdateCoroutine(IEnumerator routine)
    {
        if (_instance == null)
            return;
        _instance.updateMicroCoroutine.AddCoroutine(routine);
    }

    public static void StartFixedUpdateCoroutine(IEnumerator routine)
    {
        if (_instance == null)
            return;
        _instance.fixedUpdateMicroCoroutine.AddCoroutine(routine);
    }

    public static void StartEndOfFrameCoroutine(IEnumerator routine)
    {
        if (_instance == null)
            return;
        _instance.endOfFrameMicroCoroutine.AddCoroutine(routine);
    }

    void Awake()
    {
        StartCoroutine(RunUpdateMicroCoroutine());
        StartCoroutine(RunFixedUpdateMicroCoroutine());
        StartCoroutine(RunEndOfFrameMicroCoroutine());
    }

    IEnumerator RunUpdateMicroCoroutine()
    {
        while (true)
        {
            yield return null;
            updateMicroCoroutine.Run();
        }
    }

    IEnumerator RunFixedUpdateMicroCoroutine()
    {
        var fu = new WaitForFixedUpdate();
        while (true)
        {
            yield return fu;
            fixedUpdateMicroCoroutine.Run();
        }
    }

    IEnumerator RunEndOfFrameMicroCoroutine()
    {
        var eof = new WaitForEndOfFrame();
        while (true)
        {
            yield return eof;
            endOfFrameMicroCoroutine.Run();
        }
    }

    //현재 이것만 사용
    internal static class YieldInstructionCache //yield return은 new대신 이렇게 사용
    {
        class FloatComparer : IEqualityComparer<float>
        {
            bool IEqualityComparer<float>.Equals(float x, float y)
            {
                return x == y;
            }
            int IEqualityComparer<float>.GetHashCode(float obj)
            {
                return obj.GetHashCode();
            }
        }

        public static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
        public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();

        private static readonly Dictionary<float, WaitForSeconds> _timeInterval = new Dictionary<float, WaitForSeconds>(new FloatComparer());

        public static WaitForSeconds WaitForSeconds(float seconds)
        {
            WaitForSeconds wfs;
            if (!_timeInterval.TryGetValue(seconds, out wfs))
                _timeInterval.Add(seconds, wfs = new WaitForSeconds(seconds));
            return wfs;
        }
        // Usage.
        //yield return YieldInstructionCache.WaitForSeconds(0.1f);
        //yield return YieldInstructionCache.WaitForSeconds(seconds);
    }
}

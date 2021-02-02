using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Linq;

public class AnimationController : MonoBehaviour
{

    [HideInInspector]
    public enum UIAnimationTypes
    {
        NoAnimation,
        Scale,
        Fade,
        //PageTransitionForward,
        //PageTransitionBack,
    }


    [Header("Animations")]
    public List<Animation> animations;


    private List<int> parentAnimationsIDs;
    private List<int> animationOnEnable;
    private bool isInitialized = false;
    void Start()
    {
        List<int> children = new List<int>();
        parentAnimationsIDs = new List<int>();
        animationOnEnable = new List<int>();
        foreach (Animation animation in animations)
        {
            parentAnimationsIDs.Add(animations.IndexOf(animation));
            if (animation.isAnimateOnStart) animationOnEnable.Add(animations.IndexOf(animation));
            foreach (int child in animation.chainedAnimationsIds)
            {
                children.Add(child);
            }

        }
        children = children.Distinct().ToList();
        parentAnimationsIDs = parentAnimationsIDs.Except(children).ToList();
        animationOnEnable = animationOnEnable.Except(children).ToList();
        isInitialized = true;

    }

    private void OnEnable()
    {
        if (!isInitialized) return;
        RunAnimations(animationOnEnable);
    }


    public void AnimateAll()
    {
        RunAnimations(parentAnimationsIDs.Except(animationOnEnable).ToList());
    }

    public void AnimateSingle(int id)
    {
        RunAnimation(id);
    }

    private void RunAnimations(List<int> animationsIDs)
    {
        foreach (int animationID in animationsIDs)
        {
            if(gameObject.activeSelf)
                StartCoroutine(RunAnimation(animationID));
        }
    }

    private IEnumerator RunAnimation(int animationID)
    {
        Animation animation = animations[animationID];
        if(animation.delay > 0) 
            yield return new WaitForSeconds(animation.delay);

        if (animation.gameObjectToAnimate == null)
            animation.gameObjectToAnimate = gameObject;

        switch (animation.animationType)
        {
            case UIAnimationTypes.Scale:
                Scale(animation);
                break;
            case UIAnimationTypes.Fade:
                Fade(animation);
                break;
        }
    }

    private void ChainAndCallBack(Animation animation)
    {
        RunAnimations(animation.chainedAnimationsIds);
        animation.customCallbacks?.Invoke();
    }

    
    private void Scale(Animation animation)
    {
        animation.gameObjectToAnimate.transform.localScale = animation.vector2From;

        LeanTween.scale(animation.gameObjectToAnimate, animation.vector2To, animation.duration).setOnComplete(() =>
        {
            ChainAndCallBack(animation);
        });
    }
    
    private void Fade(Animation animation)
    {
        animation.gameObjectToAnimate.GetComponent<CanvasGroup>().alpha = animation.floatFrom;

        LeanTween.alphaCanvas(animation.gameObjectToAnimate.GetComponent<CanvasGroup>(), animation.floatTo, animation.duration).setOnComplete(() =>
        {
            ChainAndCallBack(animation);
        });
    }

    [Serializable]
    public class Animation
    {
        public bool isAnimateOnStart;
        public float delay;
        public GameObject gameObjectToAnimate;
        public UIAnimationTypes animationType;
        public LeanTweenType easeType;


        [Header("Parameters")]
        [ConditionalEnumHide("animationType", new int[] { (int)UIAnimationTypes.Scale, (int)UIAnimationTypes.Fade })] public float duration;
        [ConditionalEnumHide("animationType", new int[] { (int)UIAnimationTypes.Scale })] public Vector2 vector2From;
        [ConditionalEnumHide("animationType", new int[] { (int)UIAnimationTypes.Scale})] public Vector2 vector2To;
        [ConditionalEnumHide("animationType", new int[] { (int)UIAnimationTypes.Fade})] public float floatFrom;
        [ConditionalEnumHide("animationType", new int[] { (int)UIAnimationTypes.Fade})] public float floatTo;


        [Header("Animations")]
        public List<int> chainedAnimationsIds;
        //public List<Animation> animations;

        [Header("Callbacks")]
        public UnityEvent customCallbacks;

        // helper to set programmatically callbacks
        public void SetCustomCallbacks(UnityAction callbackAction)
        {
            UnityEvent animationCallback = new UnityEvent();

            animationCallback.AddListener(callbackAction);

            customCallbacks = animationCallback;
        }

        public override string ToString()
        {
            return JsonUtility.ToJson(this, true);
        }

    }
}

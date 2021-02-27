using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
//using UnityEngine.Rendering.PostProcessing;
// using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EffectCamera : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;

    private float cutoff = 1f, targetCutoff = 1f;
	private float prevCutoff = 1f;
	private float cutoffPos;
	private float transitionTime = 0.5f;

    public Volume ppVolume;
	private float chromaAmount, splitAmount;
    private float defaultLensDistortion;
    private float bulgeAmount;
    private float bulgeSpeed;
	private float chromaSpeed = 1f;
    private float splitSpeed = 1f;
    private float colorAmount, colorSpeed = 1f;

    private float shakeAmount = 0f, shakeTime = 0f;
    private float totalShakeTime;

	private Vector3 originalPos;

    private ChromaticAberration ca;
    private LensDistortion ld;
    //private ColorSplit cs;
    private ColorAdjustments cg;
    private float camOriginalY;

	void Start() {

        if (ppVolume)
        {
            ppVolume.profile.TryGet(out ca);
            ppVolume.profile.TryGet(out ld);
            ppVolume.profile.TryGet(out cg);
            //ppVolume.profile.TryGet(out cs);

            bulgeAmount = defaultLensDistortion = ld.intensity.value;
        }

        originalPos = virtualCamera.transform.position;

        camOriginalY = virtualCamera.transform.position.y;
    }

	void Update() {
        // chromatic aberration update
        if (ppVolume)
        {
            chromaAmount = Mathf.MoveTowards(chromaAmount, 0, Time.deltaTime * chromaSpeed);
            ca.intensity.value = chromaAmount * 0.7f * 3f;

            bulgeAmount = Mathf.MoveTowards(bulgeAmount, defaultLensDistortion, Time.deltaTime * bulgeSpeed);
            ld.intensity.value = bulgeAmount;

            //splitAmount = Mathf.MoveTowards(splitAmount, 0, Time.deltaTime * splitSpeed);
            //cs.amount.value = splitAmount * 2f;

            colorAmount = Mathf.MoveTowards(colorAmount, 0, Time.deltaTime * colorSpeed * 0.2f);
            cg.saturation.value = Mathf.Lerp(0f, 50f, colorAmount);
            cg.contrast.value = Mathf.Lerp(0f, 100f, colorAmount);
        }

        Time.timeScale = Mathf.MoveTowards(Time.timeScale, 1f, Time.unscaledDeltaTime);

        if (shakeTime > 0f)
        {
            if (Random.value < 0.3f)
                return;

            var mod = Mathf.SmoothStep(0f, 1f, shakeTime / totalShakeTime);
            shakeTime -= Time.deltaTime;

            var diff = new Vector3(Random.Range(-shakeAmount, shakeAmount) * mod, Random.Range(-shakeAmount, shakeAmount) * mod, 0);
            
            var rot = Quaternion.Euler(0, 0, Random.Range(-shakeAmount, shakeAmount) * mod * 1.5f);
            var pos = virtualCamera.transform.position;
            pos.y = camOriginalY;
            virtualCamera.ForceCameraPosition(pos + diff * 0.075f, rot);
            
            // transform.localPosition += diff * 0.02f;
            // transform.rotation = rot;
        }
        else
        {
            var p = Vector3.MoveTowards(virtualCamera.transform.position, originalPos, Time.deltaTime * 10f);
            var r = Quaternion.RotateTowards(virtualCamera.transform.rotation, Quaternion.identity, Time.deltaTime);
            virtualCamera.ForceCameraPosition(p, r);
        }
    }

	public void Chromate(float amount, float speed) {
		chromaAmount = amount;
		chromaSpeed = speed;

        splitAmount = amount * 0.005f;
        splitSpeed = speed * 0.005f;
    }

	public void Shake(float amount, float time) {
        shakeAmount = amount;
		shakeTime = time;
        totalShakeTime = time;
    }

    public void Bulge(float amount, float speed)
    {
        bulgeAmount = amount;
        bulgeSpeed = speed;
    }

    public void Decolor(float amount, float speed)
    {
        colorAmount = amount;
        colorSpeed = speed;
    }

	public void BaseEffect(float mod = 1f) {
        //impulseSource.GenerateImpulse(Vector3.one * mod * 1000f);
        Shake(2.5f * mod, 0.8f * mod);
        Chromate(0.5f * mod, 0.5f * mod);
        Bulge(defaultLensDistortion * 2f * mod, 1f * mod);
        Decolor(0.5f * mod, 3f * mod);

        //Time.timeScale = Mathf.Clamp(1f - 0.2f * mod, 0f, 1f);
    }
}
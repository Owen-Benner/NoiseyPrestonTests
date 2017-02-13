using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AsyncSoundEmitter : MonoBehaviour {

    // Logger, and related variables
    private SoundLogger logger;
    private readonly string configFileName = "soundconfig.txt";
    private readonly string outputFileName = "soundoutput.xml";

    private AudioSource myaudio;

    public void Start(){
        logger = new SoundLogger(outputFileName);
        myaudio = GetComponent<AudioSource>();

        SoundConfigInfo info = SoundConfigReader.CreateConfig(configFileName);
        print(string.Format("lo:{0}, hi:{1}", info.lowTime, info.hiTime));
        StartCoroutine(PlaySounds(info.lowTime, info.hiTime));
    }

    public IEnumerator PlaySounds(float low, float high){
        // Play sounds forever, starting immediatly, delayed uniform time between low and high
        while(true){
            myaudio.Play();
            logger.LogSound(Time.time);
            yield return new WaitForSeconds(RandomHelper.Uniform(low, high));
        }
    }
    
    void OnDestroy(){
        logger.Dispose();
    }
}


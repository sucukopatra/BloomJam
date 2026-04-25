using System.Collections;
using BloomJam.Enemies;
using UnityEngine;
using YigitcanCaliskan.EventBus;

[RequireComponent(typeof(Renderer))]
public class WallScript : MonoBehaviour
{
    [SerializeField] private EnemyType triggerType;
    [SerializeField] private int pairingId;
    [SerializeField] private Material my_material;
    [SerializeField] private Texture texture_start;
    [SerializeField] private Texture[] texture_frames;
    [SerializeField] private Texture texture_finish;
    [SerializeField] private float frames_speed;
    

    private Renderer _renderer;
    private bool _alreadyActivated;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        my_material = _renderer.material;
    } 

    private void OnEnable()  => EventBus.Subscribe<EnemyDiedEvent>(OnEnemyDied);
    private void OnDisable() => EventBus.Unsubscribe<EnemyDiedEvent>(OnEnemyDied);

    private void OnEnemyDied(EnemyDiedEvent evt)
    {
        if (_alreadyActivated) return;
        if (evt.Type != triggerType) return;
        if (evt.PairingId != pairingId) return;

        _alreadyActivated = true;


        StartCoroutine(play_frames());

    }


   private IEnumerator play_frames()
    {

        foreach (var frames in texture_frames)
        {
            my_material.mainTexture=frames;
            
            yield return new WaitForSeconds(frames_speed);


        }
        
        
        my_material.mainTexture = texture_finish;
    }
    
    
}

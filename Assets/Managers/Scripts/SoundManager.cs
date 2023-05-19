using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using Gotchi.Events;
using PhaseManager;
using Gotchi.Lickquidator.Manager;

namespace Gotchi.Audio
{
    public class SoundManager : MonoBehaviour
    {
        #region Public Variables
        public static SoundManager Instance;

        public enum SoundType
        {
            PrepPhaseMusic,
            SurvivalPhaseMusic,
            PhaseTransition,
            BasicTowerFired,
            BasicTowerHit,
            BasicTowerDied,
            BombTowerFired,
            BombTowerHit,
            BombTowerDied,
            SlowTowerFired,
            SlowTowerDied,
            TongueFired,
            AerialFired,
            PawnFired,
            GotchiSwordFired,
            GotchiSpinFired,
            GotchiDied,
            ArrowTowerFired,
            ArrowTowerHit,
            FireTowerFired,
            FireTowerHit,
            IceTowerFired,
            IceTowerHit,
            BuildingFired,
            BuildingStarted,
            MenuItemSelectedLong,
            MenuItemSelectedShort,
        }
        #endregion

        #region Fields
        [Header("Required Refs")]
        [SerializeField] private SoundTypeToAudioClip[] audioClips = null;

        [Header("Attributes")]
        [SerializeField] private int audioPoolSize = 20;
        #endregion

        #region Private Variables
        [Serializable]
        private struct SoundTypeToAudioClip
        {
            public SoundType soundType;
            public AudioClip audioClip;
        }

        private List<AudioSource> audioPool = new List<AudioSource>();
        #endregion

        #region Unity Functions
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            createAudioPool();
        }

        void OnEnable()
        {
            EventBus.MenuEvents.MenuItemSelectedLong += playMenuItemSelectedLong;
            EventBus.MenuEvents.MenuItemSelectedShort += playMenuItemSelectedShort;

            EventBus.PhaseEvents.MainMenuStarted += playPrepPhaseMusic;
            EventBus.PhaseEvents.PrepPhaseStarted += playPrepPhaseMusic;
            EventBus.PhaseEvents.SurvivalPhaseStarted += playSurvivalPhaseMusic;
            EventBus.PhaseEvents.TransitionPhaseStarted += playPhaseTransition;

            EventBus.GotchiEvents.GotchiAttacked += playGotchiAttacked;
            EventBus.GotchiEvents.GotchiDied += playGotchiDied;

            EventBus.EnemyEvents.EnemyStarted += playEnemyStarted;
            EventBus.EnemyEvents.EnemyFinished += playEnemyFinished;
            EventBus.EnemyEvents.EnemyAttacked += playEnemyAttacked;
            EventBus.EnemyEvents.EnemyDied += playEnemyDied;

            EventBus.TowerEvents.TowerStarted += playTowerStarted;
            EventBus.TowerEvents.TowerFinished += playTowerFinished;
            EventBus.TowerEvents.TowerAttacked += playTowerAttacked;
            EventBus.TowerEvents.TowerHit += playTowerHit;
            EventBus.TowerEvents.TowerDied += playTowerDied;
        }

        void OnDisable()
        {
            EventBus.MenuEvents.MenuItemSelectedLong -= playMenuItemSelectedLong;
            EventBus.MenuEvents.MenuItemSelectedShort -= playMenuItemSelectedShort;

            EventBus.PhaseEvents.MainMenuStarted -= playPrepPhaseMusic;
            EventBus.PhaseEvents.PrepPhaseStarted -= playPrepPhaseMusic;
            EventBus.PhaseEvents.SurvivalPhaseStarted -= playSurvivalPhaseMusic;
            EventBus.PhaseEvents.TransitionPhaseStarted -= playPhaseTransition;

            EventBus.GotchiEvents.GotchiAttacked -= playGotchiAttacked;
            EventBus.GotchiEvents.GotchiDied -= playGotchiDied;

            EventBus.EnemyEvents.EnemyStarted -= playEnemyStarted;
            EventBus.EnemyEvents.EnemyFinished -= playEnemyFinished;
            EventBus.EnemyEvents.EnemyAttacked -= playEnemyAttacked;
            EventBus.EnemyEvents.EnemyDied -= playEnemyDied;

            EventBus.TowerEvents.TowerStarted -= playTowerStarted;
            EventBus.TowerEvents.TowerFinished -= playTowerFinished;
            EventBus.TowerEvents.TowerAttacked -= playTowerAttacked;
            EventBus.TowerEvents.TowerHit -= playTowerHit;
            EventBus.TowerEvents.TowerDied -= playTowerDied;
        }
        #endregion

        #region Private Functions
        private void createAudioPool()
        {
            for (int i = 0; i < audioPoolSize; i++)
            {
                GameObject child = new GameObject("AudioSource_" + i);
                child.transform.SetParent(transform, true);

                AudioSource audioSource = child.AddComponent<AudioSource>();
                audioSource.spatialBlend = 1.0f;
                audioSource.playOnAwake = false;
                audioSource.maxDistance = 200;
                audioSource.rolloffMode = AudioRolloffMode.Linear;
                audioSource.dopplerLevel = 0f;
                audioSource.gameObject.SetActive(false);

                audioPool.Add(audioSource);
            }
        }

        private void playSound(SoundType soundType, bool loop = false, float volume = 0.7f)
        {
            AudioClip audioClip = null;
            for (int i = 0; i < audioClips.Length; i++)
            {
                if (audioClips[i].soundType == soundType)
                {
                    audioClip = audioClips[i].audioClip;
                    break;
                }
            }

            if (audioClip == null) return;

            foreach (AudioSource audioSource in audioPool)
            {
                bool isAudioNotAvailable = audioSource.isPlaying;
                if (isAudioNotAvailable) continue;

                audioSource.clip = audioClip;
                audioSource.volume = volume;
                audioSource.transform.position = transform.position;
                audioSource.loop = loop;
                audioSource.gameObject.SetActive(true);
                audioSource.Play();

                StartCoroutine(deactivateAudioSourceAfterDelay(audioSource, audioClip.length));

                return;
            }
        }

        private IEnumerator deactivateAudioSourceAfterDelay(AudioSource audioSource, float delay)
        {
            yield return new WaitForSeconds(delay);
            audioSource.gameObject.SetActive(false);
        }

        private void pauseSound(SoundType soundType)
        {
            AudioClip audioClip = null;
            for (int i = 0; i < audioClips.Length; i++)
            {
                if (audioClips[i].soundType == soundType)
                {
                    audioClip = audioClips[i].audioClip;
                    break;
                }
            }

            if (audioClip == null) return;

            foreach (AudioSource audioSource in audioPool)
            {
                if (audioSource.clip == audioClip && audioSource.isPlaying)
                {
                    audioSource.Stop();
                    audioSource.gameObject.SetActive(false);
                    return;
                }
            }
        }

        private void playMenuItemSelectedLong()
        {
            playSound(SoundManager.SoundType.MenuItemSelectedLong);
        }

        private void playMenuItemSelectedShort()
        {
            playSound(SoundManager.SoundType.MenuItemSelectedShort);
        }

        private void playPrepPhaseMusic()
        {
            pauseSound(SoundManager.SoundType.PrepPhaseMusic);
            playSound(SoundManager.SoundType.PrepPhaseMusic, true);
        }

        private void playSurvivalPhaseMusic()
        {
            pauseSound(SoundManager.SoundType.SurvivalPhaseMusic);
            playSound(SoundManager.SoundType.SurvivalPhaseMusic, true);
        }

        private void playPhaseTransition(Phase nextPhase)
        {
            pauseSound(SoundManager.SoundType.PrepPhaseMusic);
            pauseSound(SoundManager.SoundType.SurvivalPhaseMusic);
            playSound(SoundManager.SoundType.PhaseTransition);
        }

        private void playGotchiAttacked(GotchiManager.AttackType attackType)
        {
            if (attackType == GotchiManager.AttackType.Basic)
            {
                playSound(SoundManager.SoundType.GotchiSwordFired);
            }
            else if (attackType == GotchiManager.AttackType.Spin)
            {
                playSound(SoundManager.SoundType.GotchiSpinFired);
            }
        }

        private void playGotchiDied()
        {
            playSound(SoundManager.SoundType.GotchiDied);
        }

        private void playEnemyStarted()
        {
            playSound(SoundManager.SoundType.BuildingStarted);
        }

        private void playEnemyFinished(EnemyBlueprint enemyBlueprint)
        {
            playSound(SoundManager.SoundType.BuildingFired);
        }

        private void playEnemyAttacked(LickquidatorManager.LickquidatorType enemyType)
        {
            if (enemyType == LickquidatorManager.LickquidatorType.PawnLickquidator)
            {
                playSound(SoundManager.SoundType.PawnFired);
            }
            else if (enemyType == LickquidatorManager.LickquidatorType.AerialLickquidator)
            {
                playSound(SoundManager.SoundType.AerialFired);
            }
            else if (enemyType == LickquidatorManager.LickquidatorType.BossLickquidator)
            {
                playSound(SoundManager.SoundType.TongueFired);
            }
        }

        private void playEnemyDied(LickquidatorManager.LickquidatorType enemyType)
        {
            playSound(SoundManager.SoundType.BasicTowerDied);
        }

        private void playTowerStarted()
        {
            playSound(SoundManager.SoundType.BuildingStarted);
        }

        private void playTowerFinished(TowerBlueprint towerBlueprint)
        {
            playSound(SoundManager.SoundType.BuildingFired);
        }

        private void playTowerAttacked(TowerPool.TowerType towerType)
        {
            if (towerType == TowerPool.TowerType.BasicTower)
            {
                playSound(SoundManager.SoundType.BasicTowerFired);
            }
            else if (towerType == TowerPool.TowerType.BombTower)
            {
                playSound(SoundManager.SoundType.BombTowerFired);
            }
            else if (towerType == TowerPool.TowerType.SlowTower)
            {
                playSound(SoundManager.SoundType.SlowTowerFired);
            }
            else if (towerType == TowerPool.TowerType.ArrowTower1 || towerType == TowerPool.TowerType.ArrowTower2 || towerType == TowerPool.TowerType.ArrowTower3)
            {
                playSound(SoundManager.SoundType.ArrowTowerFired);
            }
            else if (towerType == TowerPool.TowerType.FireTower1 || towerType == TowerPool.TowerType.FireTower2 || towerType == TowerPool.TowerType.FireTower3)
            {
                playSound(SoundManager.SoundType.FireTowerFired);
            }
            else if (towerType == TowerPool.TowerType.IceTower1 || towerType == TowerPool.TowerType.IceTower2 || towerType == TowerPool.TowerType.IceTower3)
            {
                playSound(SoundManager.SoundType.IceTowerFired);
            }
        }

        private void playTowerHit(TowerPool.TowerType towerType)
        {
           if (towerType == TowerPool.TowerType.BasicTower)
            {
                playSound(SoundManager.SoundType.BasicTowerHit);
            }
            else if (towerType == TowerPool.TowerType.BombTower)
            {
                playSound(SoundManager.SoundType.BombTowerHit);
            }
            else if (towerType == TowerPool.TowerType.ArrowTower1 || towerType == TowerPool.TowerType.ArrowTower2 || towerType == TowerPool.TowerType.ArrowTower3)
            {
                playSound(SoundManager.SoundType.ArrowTowerHit);
            }
            else if (towerType == TowerPool.TowerType.FireTower1 || towerType == TowerPool.TowerType.FireTower2 || towerType == TowerPool.TowerType.FireTower3)
            {
                playSound(SoundManager.SoundType.FireTowerHit);
            }
            else if (towerType == TowerPool.TowerType.IceTower1 || towerType == TowerPool.TowerType.IceTower2 || towerType == TowerPool.TowerType.IceTower3)
            {
                playSound(SoundManager.SoundType.IceTowerHit);
            }
        }

        private void playTowerDied(TowerPool.TowerType towerType)
        {
            playSound(SoundManager.SoundType.BasicTowerDied);
        }
        #endregion
    }
}
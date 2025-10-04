// Serviço de efeitos sonoros
class SoundService {
  private audioContext: AudioContext | null = null;
  private sounds: Map<string, AudioBuffer> = new Map();
  private isEnabled: boolean = true;
  private volume: number = 0.5;

  constructor() {
    this.initializeAudioContext();
  }

  private async initializeAudioContext() {
    try {
      this.audioContext = new (window.AudioContext || (window as any).webkitAudioContext)();
    } catch (error) {
      console.warn('Web Audio API não suportada:', error);
    }
  }

  // Criar sons sintéticos
  private createTone(frequency: number, duration: number, type: OscillatorType = 'sine'): AudioBuffer | null {
    if (!this.audioContext) return null;

    const sampleRate = this.audioContext.sampleRate;
    const buffer = this.audioContext.createBuffer(1, sampleRate * duration, sampleRate);
    const data = buffer.getChannelData(0);

    for (let i = 0; i < data.length; i++) {
      const t = i / sampleRate;
      let value = 0;

      switch (type) {
        case 'sine':
          value = Math.sin(2 * Math.PI * frequency * t);
          break;
        case 'square':
          value = Math.sin(2 * Math.PI * frequency * t) > 0 ? 1 : -1;
          break;
        case 'sawtooth':
          value = 2 * (t * frequency - Math.floor(t * frequency + 0.5));
          break;
        case 'triangle':
          value = 2 * Math.abs(2 * (t * frequency - Math.floor(t * frequency + 0.5))) - 1;
          break;
      }

      // Aplicar envelope para evitar clicks
      const envelope = Math.exp(-t * 3);
      data[i] = value * envelope * this.volume;
    }

    return buffer;
  }

  // Sons do jogo
  private async loadSounds() {
    if (!this.audioContext) return;

    // Som de clique/UI
    this.sounds.set('click', this.createTone(800, 0.1, 'square')!);
    
    // Som de sucesso
    this.sounds.set('success', this.createTone(600, 0.2, 'sine')!);
    
    // Som de erro
    this.sounds.set('error', this.createTone(200, 0.3, 'sawtooth')!);
    
    // Som de combate - ataque
    this.sounds.set('attack', this.createTone(400, 0.15, 'square')!);
    
    // Som de combate - crítico
    this.sounds.set('critical', this.createTone(800, 0.2, 'triangle')!);
    
    // Som de level up
    this.sounds.set('levelup', this.createTone(600, 0.5, 'sine')!);
    
    // Som de conquista
    this.sounds.set('achievement', this.createTone(1000, 0.3, 'triangle')!);
    
    // Som de moedas
    this.sounds.set('coins', this.createTone(1200, 0.1, 'square')!);
    
    // Som de cura
    this.sounds.set('heal', this.createTone(500, 0.2, 'sine')!);
    
    // Som de dano
    this.sounds.set('damage', this.createTone(300, 0.1, 'sawtooth')!);
  }

  async play(soundName: string): Promise<void> {
    if (!this.isEnabled || !this.audioContext) return;

    try {
      // Carregar sons se ainda não foram carregados
      if (this.sounds.size === 0) {
        await this.loadSounds();
      }

      const buffer = this.sounds.get(soundName);
      if (!buffer) {
        console.warn(`Som '${soundName}' não encontrado`);
        return;
      }

      const source = this.audioContext.createBufferSource();
      const gainNode = this.audioContext.createGain();
      
      source.buffer = buffer;
      gainNode.gain.value = this.volume;
      
      source.connect(gainNode);
      gainNode.connect(this.audioContext.destination);
      
      source.start();
    } catch (error) {
      console.warn('Erro ao reproduzir som:', error);
    }
  }

  // Métodos específicos para diferentes ações do jogo
  async playSound(soundName: string): Promise<void> {
    await this.play(soundName);
  }

  async playClick(): Promise<void> {
    await this.play('click');
  }

  async playSuccess(): Promise<void> {
    await this.play('success');
  }

  async playError(): Promise<void> {
    await this.play('error');
  }

  async playAttack(): Promise<void> {
    await this.play('attack');
  }

  async playCritical(): Promise<void> {
    await this.play('critical');
  }

  async playLevelUp(): Promise<void> {
    await this.play('levelup');
  }

  async playAchievement(): Promise<void> {
    await this.play('achievement');
  }

  async playCoins(): Promise<void> {
    await this.play('coins');
  }

  async playHeal(): Promise<void> {
    await this.play('heal');
  }

  async playDamage(): Promise<void> {
    await this.play('damage');
  }

  // Controles de áudio
  setEnabled(enabled: boolean): void {
    this.isEnabled = enabled;
    localStorage.setItem('soundEnabled', enabled.toString());
  }

  setVolume(volume: number): void {
    this.volume = Math.max(0, Math.min(1, volume));
    localStorage.setItem('soundVolume', this.volume.toString());
  }

  isSoundEnabled(): boolean {
    return this.isEnabled;
  }

  getVolume(): number {
    return this.volume;
  }

  // Inicializar configurações salvas
  initializeSettings(): void {
    const savedEnabled = localStorage.getItem('soundEnabled');
    const savedVolume = localStorage.getItem('soundVolume');

    if (savedEnabled !== null) {
      this.isEnabled = savedEnabled === 'true';
    }

    if (savedVolume !== null) {
      this.volume = parseFloat(savedVolume);
    }
  }
}

export const soundService = new SoundService();

// Inicializar configurações ao carregar
soundService.initializeSettings();

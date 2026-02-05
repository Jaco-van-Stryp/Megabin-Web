import { Injectable, signal, OnDestroy } from '@angular/core';

/**
 * Service for tracking responsive breakpoints
 * Uses a single breakpoint at 768px to distinguish mobile from desktop
 */
@Injectable({
  providedIn: 'root',
})
export class ResponsiveService implements OnDestroy {
  private readonly MOBILE_BREAKPOINT = 768;
  private resizeListener: (() => void) | null = null;

  private isMobileSignal = signal(this.checkIsMobile());

  /** Readonly signal indicating if the current viewport is mobile (<768px) */
  readonly isMobile = this.isMobileSignal.asReadonly();

  constructor() {
    if (typeof window !== 'undefined') {
      this.resizeListener = () => this.updateScreenSize();
      window.addEventListener('resize', this.resizeListener);
    }
  }

  ngOnDestroy(): void {
    if (typeof window !== 'undefined' && this.resizeListener) {
      window.removeEventListener('resize', this.resizeListener);
    }
  }

  private checkIsMobile(): boolean {
    if (typeof window !== 'undefined') {
      return window.innerWidth < this.MOBILE_BREAKPOINT;
    }
    return false;
  }

  private updateScreenSize(): void {
    this.isMobileSignal.set(this.checkIsMobile());
  }
}

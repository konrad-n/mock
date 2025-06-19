// MAUI App Theme Configuration
// Based on the original MAUI app screenshots

export const mauiTheme = {
  colors: {
    // Primary colors
    primary: '#00BCD4', // Turquoise/Cyan
    primaryDark: '#0097A7',
    primaryLight: '#B2EBF2',
    
    // Background colors
    background: '#F5F5F5', // Light gray background
    cardBackground: '#FFFFFF', // White cards
    
    // Text colors
    textPrimary: '#212121', // Almost black
    textSecondary: '#757575', // Gray
    textOnPrimary: '#FFFFFF', // White text on primary color
    
    // Status colors
    success: '#4CAF50',
    warning: '#FFC107',
    error: '#F44336',
    info: '#2196F3',
    
    // UI element colors
    divider: '#E0E0E0',
    disabled: '#BDBDBD',
    
    // Progress colors
    progressBackground: '#E0E0E0',
    progressFill: '#00BCD4',
    
    // Icon colors
    iconActive: '#212121',
    iconInactive: '#757575',
    
    // Module switcher colors
    moduleActive: '#00BCD4',
    moduleInactive: '#FFFFFF',
    moduleActiveBorder: '#00BCD4',
    moduleInactiveBorder: '#E0E0E0',
  },
  
  typography: {
    fontFamily: '-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif',
    
    // Font sizes
    h1: {
      fontSize: '24px',
      fontWeight: '500',
      lineHeight: '32px',
    },
    h2: {
      fontSize: '20px',
      fontWeight: '500',
      lineHeight: '28px',
    },
    h3: {
      fontSize: '18px',
      fontWeight: '500',
      lineHeight: '24px',
    },
    body1: {
      fontSize: '16px',
      fontWeight: '400',
      lineHeight: '24px',
    },
    body2: {
      fontSize: '14px',
      fontWeight: '400',
      lineHeight: '20px',
    },
    caption: {
      fontSize: '12px',
      fontWeight: '400',
      lineHeight: '16px',
    },
    button: {
      fontSize: '16px',
      fontWeight: '500',
      lineHeight: '24px',
      textTransform: 'none' as const,
    },
  },
  
  spacing: {
    xs: '4px',
    sm: '8px',
    md: '16px',
    lg: '24px',
    xl: '32px',
    xxl: '48px',
  },
  
  borderRadius: {
    sm: '8px',
    md: '12px',
    lg: '16px',
    xl: '24px',
    full: '9999px',
  },
  
  shadows: {
    card: '0 2px 4px rgba(0, 0, 0, 0.1)',
    button: '0 2px 8px rgba(0, 188, 212, 0.3)',
    elevated: '0 4px 12px rgba(0, 0, 0, 0.15)',
  },
  
  components: {
    // Navigation drawer
    drawer: {
      width: '280px',
      backgroundColor: '#FFFFFF',
      itemHeight: '48px',
      itemPadding: '16px',
      itemActiveBackground: '#E0F7FA',
      itemHoverBackground: '#F5F5F5',
    },
    
    // Cards
    card: {
      padding: '16px',
      borderRadius: '12px',
      backgroundColor: '#FFFFFF',
      shadow: '0 2px 4px rgba(0, 0, 0, 0.1)',
    },
    
    // Buttons
    button: {
      height: '48px',
      padding: '12px 24px',
      borderRadius: '24px',
      primaryBackground: '#00BCD4',
      primaryHover: '#0097A7',
      primaryTextColor: '#FFFFFF',
      secondaryBackground: '#FFFFFF',
      secondaryHover: '#F5F5F5',
      secondaryTextColor: '#00BCD4',
      secondaryBorder: '1px solid #00BCD4',
    },
    
    // Progress bars
    progressBar: {
      height: '8px',
      borderRadius: '4px',
      backgroundColor: '#E0E0E0',
      fillColor: '#00BCD4',
    },
    
    // Module switcher
    moduleSwitcher: {
      height: '48px',
      borderRadius: '24px',
      backgroundColor: '#FFFFFF',
      border: '1px solid #E0E0E0',
      activeTabBackground: '#00BCD4',
      activeTabTextColor: '#FFFFFF',
      inactiveTabTextColor: '#757575',
    },
    
    // Status bar (top)
    statusBar: {
      height: '56px',
      backgroundColor: '#00BCD4',
      textColor: '#FFFFFF',
    },
    
    // Bottom navigation
    bottomNav: {
      height: '56px',
      backgroundColor: '#FFFFFF',
      borderTop: '1px solid #E0E0E0',
      iconSize: '24px',
    },
    
    // Form inputs
    input: {
      height: '48px',
      padding: '12px 16px',
      borderRadius: '8px',
      borderColor: '#E0E0E0',
      borderColorFocus: '#00BCD4',
      backgroundColor: '#FFFFFF',
      placeholderColor: '#9E9E9E',
    },
    
    // Stats cards (like Staże 1/12, Dyżury 0/1048h)
    statsCard: {
      minHeight: '120px',
      padding: '16px',
      textAlign: 'center' as const,
      titleFontSize: '14px',
      titleColor: '#757575',
      valueFontSize: '24px',
      valueColor: '#212121',
      valueFontWeight: '500',
    },
  },
  
  animations: {
    duration: {
      fast: '150ms',
      normal: '300ms',
      slow: '500ms',
    },
    easing: {
      standard: 'cubic-bezier(0.4, 0, 0.2, 1)',
      decelerate: 'cubic-bezier(0, 0, 0.2, 1)',
      accelerate: 'cubic-bezier(0.4, 0, 1, 1)',
    },
  },
};

export type MauiTheme = typeof mauiTheme;
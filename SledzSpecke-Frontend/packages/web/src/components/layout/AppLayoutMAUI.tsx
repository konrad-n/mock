import { useState } from 'react';
import { Outlet, useNavigate, useLocation } from 'react-router-dom';
import { mauiTheme } from '@/theme/mauiTheme';
import { useAuthStore } from '@/stores/authStore';

interface NavItem {
  label: string;
  path: string;
  icon: string;
}

const navItems: NavItem[] = [
  { label: 'Dashboard', path: '/dashboard', icon: '⊞' },
  { label: 'Procedury', path: '/procedures', icon: '≡' },
  { label: 'Dyżury', path: '/medical-shifts', icon: '🌙' },
  { label: 'Staże', path: '/internships', icon: '👨‍⚕️' },
  { label: 'Kursy', path: '/courses', icon: '🎓' },
  { label: 'Samokształcenie', path: '/self-education', icon: '📚' },
  { label: 'Publikacje', path: '/publications', icon: '📄' },
  { label: 'Nieobecności', path: '/absences', icon: '📅' },
  { label: 'Eksport', path: '/export', icon: '📥' },
  { label: 'Ustawienia', path: '/settings', icon: '⚙️' },
];

export const AppLayoutMAUI = () => {
  const [drawerOpen, setDrawerOpen] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();
  const logout = useAuthStore((state) => state.logout);

  const handleNavigation = (path: string) => {
    navigate(path);
    setDrawerOpen(false);
  };

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const styles = {
    container: {
      display: 'flex',
      minHeight: '100vh',
      backgroundColor: mauiTheme.colors.background,
    },
    drawer: {
      position: 'fixed' as const,
      top: 0,
      left: drawerOpen ? 0 : '-280px',
      width: '280px',
      height: '100vh',
      backgroundColor: mauiTheme.colors.cardBackground,
      boxShadow: mauiTheme.shadows.elevated,
      transition: 'left 0.3s ease',
      zIndex: 1100,
      display: 'flex',
      flexDirection: 'column' as const,
    },
    drawerHeader: {
      height: '160px',
      backgroundColor: mauiTheme.colors.primary,
      color: mauiTheme.colors.textOnPrimary,
      display: 'flex',
      flexDirection: 'column' as const,
      alignItems: 'center',
      justifyContent: 'center',
      position: 'relative' as const,
    },
    drawerLogo: {
      width: '80px',
      height: '80px',
      backgroundColor: mauiTheme.colors.cardBackground,
      borderRadius: '50%',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      marginBottom: '12px',
      fontSize: '36px',
    },
    drawerTitle: {
      fontSize: '18px',
      fontWeight: '500',
    },
    drawerSubtitle: {
      fontSize: '14px',
      opacity: 0.8,
    },
    navList: {
      flex: 1,
      overflowY: 'auto' as const,
      padding: '8px 0',
    },
    navItem: {
      display: 'flex',
      alignItems: 'center',
      padding: '12px 16px',
      cursor: 'pointer',
      transition: 'background-color 0.2s ease',
      color: mauiTheme.colors.textPrimary,
      textDecoration: 'none',
      fontSize: '16px',
    },
    navItemActive: {
      backgroundColor: '#E0F7FA',
      color: mauiTheme.colors.primary,
    },
    navItemHover: {
      backgroundColor: mauiTheme.colors.background,
    },
    navIcon: {
      width: '24px',
      marginRight: '16px',
      textAlign: 'center' as const,
      fontSize: '20px',
    },
    logoutButton: {
      borderTop: `1px solid ${mauiTheme.colors.divider}`,
      padding: '16px',
    },
    logoutButtonInner: {
      width: '100%',
      padding: '12px',
      backgroundColor: mauiTheme.colors.primary,
      color: mauiTheme.colors.textOnPrimary,
      border: 'none',
      borderRadius: mauiTheme.borderRadius.xl,
      fontSize: '16px',
      fontWeight: '500',
      cursor: 'pointer',
      transition: 'background-color 0.3s ease',
    },
    main: {
      flex: 1,
      display: 'flex',
      flexDirection: 'column' as const,
      transition: 'margin-left 0.3s ease',
    },
    header: {
      height: '56px',
      backgroundColor: mauiTheme.colors.primary,
      color: mauiTheme.colors.textOnPrimary,
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'space-between',
      padding: '0 16px',
      position: 'sticky' as const,
      top: 0,
      zIndex: 1000,
    },
    menuButton: {
      background: 'none',
      border: 'none',
      color: 'white',
      fontSize: '24px',
      cursor: 'pointer',
      padding: '8px',
    },
    headerTitle: {
      fontSize: '20px',
      fontWeight: '500',
    },
    content: {
      flex: 1,
      overflowY: 'auto' as const,
    },
    overlay: {
      position: 'fixed' as const,
      top: 0,
      left: 0,
      right: 0,
      bottom: 0,
      backgroundColor: 'rgba(0, 0, 0, 0.5)',
      display: drawerOpen ? 'block' : 'none',
      zIndex: 1050,
    },
  };

  return (
    <div style={styles.container}>
      {/* Overlay */}
      <div style={styles.overlay} onClick={() => setDrawerOpen(false)} />

      {/* Drawer */}
      <nav style={styles.drawer}>
        <div style={styles.drawerHeader}>
          <div style={styles.drawerLogo}>🐟</div>
          <div style={styles.drawerTitle}>SledzSpecke</div>
          <div style={styles.drawerSubtitle}>old</div>
          <div style={styles.drawerSubtitle}>Kardiologia</div>
        </div>

        <div style={styles.navList}>
          {navItems.map((item) => (
            <div
              key={item.path}
              style={{
                ...styles.navItem,
                ...(location.pathname === item.path ? styles.navItemActive : {}),
              }}
              onClick={() => handleNavigation(item.path)}
              onMouseEnter={(e) => {
                if (location.pathname !== item.path) {
                  e.currentTarget.style.backgroundColor = styles.navItemHover.backgroundColor;
                }
              }}
              onMouseLeave={(e) => {
                if (location.pathname !== item.path) {
                  e.currentTarget.style.backgroundColor = 'transparent';
                }
              }}
            >
              <span style={styles.navIcon}>{item.icon}</span>
              {item.label}
            </div>
          ))}
        </div>

        <div style={styles.logoutButton}>
          <button
            style={styles.logoutButtonInner}
            onClick={handleLogout}
            onMouseEnter={(e) => e.currentTarget.style.backgroundColor = mauiTheme.colors.primaryDark}
            onMouseLeave={(e) => e.currentTarget.style.backgroundColor = mauiTheme.colors.primary}
          >
            Wyloguj
          </button>
        </div>
      </nav>

      {/* Main content */}
      <div style={styles.main}>
        <header style={styles.header}>
          <button
            style={styles.menuButton}
            onClick={() => setDrawerOpen(!drawerOpen)}
          >
            ☰
          </button>
          <span style={styles.headerTitle}>SledzSpecke</span>
          <div style={{ width: '40px' }}></div>
        </header>

        <main style={styles.content}>
          <Outlet />
        </main>
      </div>
    </div>
  );
};
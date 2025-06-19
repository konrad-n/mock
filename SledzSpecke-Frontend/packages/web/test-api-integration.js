#!/usr/bin/env node

import axios from 'axios';

const API_BASE_URL = 'http://localhost:5000/api';
const TEST_TOKEN = 'your-test-token'; // We'll get this from login

const endpoints = [
  { method: 'GET', url: '/MedicalShifts', auth: true },
  { method: 'GET', url: '/Procedures', auth: true },
  { method: 'GET', url: '/Internships', auth: true },
  { method: 'GET', url: '/Courses', auth: true },
  { method: 'GET', url: '/SelfEducation', auth: true },
  { method: 'POST', url: '/auth/sign-in', auth: false, data: { Username: 'testuser', Password: 'Test123!' } },
];

async function testEndpoints() {
  console.log('Testing API endpoints...\n');
  
  let token = null;
  
  for (const endpoint of endpoints) {
    const url = `${API_BASE_URL}${endpoint.url}`;
    const config = {
      method: endpoint.method,
      url,
      headers: {},
      data: endpoint.data,
    };
    
    if (endpoint.auth && token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    
    try {
      const response = await axios(config);
      console.log(`✅ ${endpoint.method} ${endpoint.url} - Status: ${response.status}`);
      
      // Save token from login
      if (endpoint.url === '/auth/sign-in' && response.data.AccessToken) {
        token = response.data.AccessToken;
        console.log('   Token obtained successfully');
      }
    } catch (error) {
      console.log(`❌ ${endpoint.method} ${endpoint.url} - Error: ${error.response?.status || error.message}`);
      if (error.response?.data) {
        console.log(`   Details: ${JSON.stringify(error.response.data).substring(0, 100)}...`);
      }
    }
  }
}

testEndpoints();
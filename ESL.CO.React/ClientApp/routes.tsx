import * as React from 'react';
import { Redirect } from 'react-router'; 
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { BoardList } from './components/BoardList';
import { KanbanBoard } from './components/KanbanBoard';
import { BoardReader } from './components/BoardReader';
import { StatisticsList } from './components/StatisticsList';
import { StatisticsBoard } from './components/StatisticsBoard';
import { Login } from './components/Login';
import { PresentationList } from './components/PresentationList';
import { NavMenu } from './components/NavMenu';
import { BoardReaderFromUrl } from './components/BoardReaderFromUrl';

export const routes = <Layout>
    <Route path='/' component={NavMenu} />
    <Route exact path='/' component={BoardReader} />
    <Route exact path='/view/:id' component={BoardReader} />

    <Route path='/statistics' component={StatisticsList} />
    <Route path="/jiraconnectionstats/:id" component={StatisticsBoard} />

    <Route exact path='/admin' component={BoardList} />
    <Route path='/admin/presentations' component={PresentationList} />
    <Route path='/admin/presentations/:id' component={BoardList} />

    <Route path='/login' component={Login} />

    <Route path='/k/:id' component={BoardReaderFromUrl} />
</Layout>;

// adjust NavMenu accordingly
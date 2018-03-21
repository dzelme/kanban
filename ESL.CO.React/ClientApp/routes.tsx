import * as React from 'react';
import { Redirect } from 'react-router'; 
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { BoardList } from './components/BoardList';
import { BoardReader } from './components/BoardReader';
import { AllBoardReader } from './components/AllBoardReader';
import { StatisticsList } from './components/StatisticsList';
import { StatisticsBoard } from './components/StatisticsBoard';
import { Login } from './components/Login';
import { PresentationList } from './components/PresentationList';
import { NavMenu } from './components/NavMenu';
import { BoardReaderFromUrl } from './components/BoardReaderFromUrl';

export const routes = <Layout>
    <Route path='/login' component={Login} />
    <Route exact path='/admin' component={PresentationList} />
    <Route path='/admin/presentations' component={PresentationList} />
    <Route path='/admin/createPresentation' component={BoardList} />
    <Route path='/admin/statistics' component={StatisticsList} />
    <Route path="/admin/jiraconnectionstats/:id" component={StatisticsBoard} />
    <Route path='/' component={NavMenu} />
    <Route exact path='/' component={BoardReader} />

    <Route exact path='/p/:id' component={BoardReader} />
    <Route exact path='/k/:id' component={BoardReaderFromUrl} />
</Layout>;
